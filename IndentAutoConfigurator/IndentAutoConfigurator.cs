using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace IndentAutoConfigurator
{
    ///<summary>
    ///</summary>
    public class IndentAutoConfigurator
    {
        //private IAdornmentLayer _layer;
        private IWpfTextView _view;

        public IndentAutoConfigurator(IWpfTextView view)
        {
            _view = view;
            //_layer = view.GetAdornmentLayer("IndentAutoConfigurator");

            UpdateSetting();
        }

        private void UpdateSetting()
        {
            CodeIndention codeIndention = new CodeIndention();
            foreach (var line in _view.TextSnapshot.Lines)
            {
                var text = line.GetText();
                if (text.StartsWith(" "))
                {
                    codeIndention = new CodeIndention { IsTab = false, IndentSize = Regex.Match(text, "^([ ]+)").Length };
                    break;
                }
                else if (text.StartsWith("\t"))
                {
                    codeIndention = new CodeIndention { IsTab = true, IndentSize = 0 };
                    break;
                }
            }

            if (codeIndention == null)
                return;

            System.Diagnostics.Debug.WriteLine(String.Format("{0}, {1}", codeIndention.IsTab, codeIndention.IndentSize));

            _view.Options.SetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId, !codeIndention.IsTab);
            if (!codeIndention.IsTab)
                _view.Options.SetOptionValue(DefaultOptions.IndentSizeOptionId, codeIndention.IndentSize);
        }

        private class CodeIndention
        {
            public Boolean IsTab { get; set; }
            public Int32 IndentSize { get; set; }
        }
    }
}
