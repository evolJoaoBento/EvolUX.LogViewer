using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using TreeNode = EvolUX.LogViewer.Models.TreeNode;

namespace EvolUX.LogViewer
{
    public partial class VariableDetailsWindow : Window
    {
        private string _formattedContent = string.Empty;

        public VariableDetailsWindow()
        {
            InitializeComponent();
        }

        public void DisplayVariable(TreeNode node)
        {
            if (node == null) return;

            variableNameText.Text = node.Key;

            // Determine the type of the value
            var rawValue = node.RawValue;
            if (rawValue != null)
            {
                variableTypeText.Text = $"Type: {GetFriendlyTypeName(rawValue.GetType())}";
                _formattedContent = FormatValue(rawValue);
            }
            else if (node.HasValue)
            {
                variableTypeText.Text = "Type: String";
                _formattedContent = node.Value ?? "null";
            }
            else if (node.Children.Count > 0)
            {
                variableTypeText.Text = "Type: Object";
                _formattedContent = FormatTreeNodeAsJson(node);
            }
            else
            {
                variableTypeText.Text = "Type: null";
                _formattedContent = "null";
            }

            contentTextBox.Text = _formattedContent;
        }

        private string GetFriendlyTypeName(Type type)
        {
            if (type == typeof(string)) return "String";
            if (type == typeof(int) || type == typeof(long)) return "Integer";
            if (type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return "Number";
            if (type == typeof(bool)) return "Boolean";
            if (type == typeof(DateTime)) return "DateTime";
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return "Object";
            if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
                return "Array";
            return type.Name;
        }

        private string FormatValue(object value)
        {
            if (value == null) return "null";

            // For simple types, just return the value
            if (value is string strVal)
            {
                // Check if it might be JSON
                if ((strVal.StartsWith("{") && strVal.EndsWith("}")) ||
                    (strVal.StartsWith("[") && strVal.EndsWith("]")))
                {
                    try
                    {
                        var parsed = JsonConvert.DeserializeObject(strVal);
                        return JsonConvert.SerializeObject(parsed, Formatting.Indented);
                    }
                    catch
                    {
                        return strVal;
                    }
                }
                return strVal;
            }

            if (value is int or long or double or float or decimal or bool)
            {
                return value.ToString() ?? "null";
            }

            if (value is DateTime dt)
            {
                return dt.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
            }

            // For complex types, serialize as JSON
            try
            {
                return JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    MaxDepth = 10
                });
            }
            catch
            {
                return value.ToString() ?? "null";
            }
        }

        private string FormatTreeNodeAsJson(TreeNode node)
        {
            var obj = TreeNodeToObject(node);
            try
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
            catch
            {
                return node.Value ?? "null";
            }
        }

        private object? TreeNodeToObject(TreeNode node)
        {
            if (node.RawValue != null)
            {
                return node.RawValue;
            }

            if (node.Children.Count == 0)
            {
                return node.Value;
            }

            // Check if children represent array indices
            bool isArray = node.Children.All(c => c.Key.StartsWith("[") && c.Key.EndsWith("]"));

            if (isArray)
            {
                var list = new List<object?>();
                foreach (var child in node.Children)
                {
                    list.Add(TreeNodeToObject(child));
                }
                return list;
            }
            else
            {
                var dict = new Dictionary<string, object?>();
                foreach (var child in node.Children)
                {
                    dict[child.Key] = TreeNodeToObject(child);
                }
                return dict;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = searchBox.Text;
            if (string.IsNullOrEmpty(searchText))
            {
                contentTextBox.Text = _formattedContent;
                return;
            }

            // Highlight search results by finding the first occurrence
            var index = _formattedContent.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                contentTextBox.Text = _formattedContent;
                contentTextBox.Focus();
                contentTextBox.Select(index, searchText.Length);
                contentTextBox.ScrollToLine(contentTextBox.GetLineIndexFromCharacterIndex(index));
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Clipboard.SetText(_formattedContent);
                System.Windows.MessageBox.Show("Content copied to clipboard.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                System.Windows.MessageBox.Show("Failed to copy to clipboard.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
