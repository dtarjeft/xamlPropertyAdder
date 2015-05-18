using System.Collections.Generic;

namespace XAMLPropertyAdder
{
    class Program
    {
        static void Main()
        {
            var dir = new DirCrawler();
            dir.GetFiles(@"C:\Users\Admin\Desktop\CSC2111\XAMLPropertyAdder\XamlAppForTesting", "*.xaml"); // TODO: Replace before you use this app!
            // This list taken from: https://msdn.microsoft.com/en-us/library/ms744822(v=vs.110).aspx
            var whites = new[]
            {
                "Button", "CheckBox", "CheckedListBox", "ColorDialog", "ComboBox",
                "FolderBrowser", "FontDialog", "GroupBox", "HscrollBar", "ImageList",
                "Label", "ListBox", "ListView", "MainMenu/ContextMenu", "MonthCalendar",
                "NotifyIcon", "OpenFileDialog", "PageSetupDialog", "PrintDialog",
                "ProgressBar", "RadioButton", "RichTextBox", "SaveFileDialog", "ScrollableControl",
                "SoundPlayer", "StatusBar", "TabControl/TabPage","TextBox", "Timer", 
                "Toolbar", "ToolTip", "TrackBar", "TreeView", "VscrollBar", "WebBrowser"
            }; // TODO: Determine which of these to actually use....

            foreach (var result in dir.Results)
            {
                var xml = new XmlParser(result, whites);
                xml.Parse("_Test2", new List<string>{"AutomationProperties.AutomationName", "AutomationProperties.AutomationId"});
            }
        }
    }
}
