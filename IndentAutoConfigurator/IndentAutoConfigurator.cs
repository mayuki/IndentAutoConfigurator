using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
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
        private IClassifierAggregatorService _classifierAggregatorService;

        public IndentAutoConfigurator(IWpfTextView view, IClassifierAggregatorService classifierAggregatorService)
        {
            _view = view;
            _classifierAggregatorService = classifierAggregatorService;
            //_layer = view.GetAdornmentLayer("IndentAutoConfigurator");

            UpdateSetting();
        }

        private void UpdateSetting()
        {
            CodeIndention codeIndention = null;
            foreach (var line in _view.TextSnapshot.Lines)
            {
                // Classification: http://code.msdn.microsoft.com/ToDoGlyphFactory-ef2db126/sourcecode?fileId=72&pathId=393561789
                var isInComment = _classifierAggregatorService.GetClassifier(_view.TextBuffer).GetClassificationSpans(line.Extent).Any(x => x.ClassificationType.Classification.ToLower().Contains("comment"));
                if (isInComment)
                    continue;

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
