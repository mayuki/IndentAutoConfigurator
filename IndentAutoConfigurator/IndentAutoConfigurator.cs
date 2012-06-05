using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Misuzilla.IndentAutoConfigurator
{
    ///<summary>
    ///</summary>
    public class IndentAutoConfigurator
    {
        //private IAdornmentLayer _layer;
        private IWpfTextView _view;
        private IClassifierAggregatorService _classifierAggregatorService;
        private IServiceProvider _serviceProvider;
        private IndentAutoConfiguratorService _service;

        private CodeIndention CurrentCodeIndention { get; set; }

        public IndentAutoConfigurator(IWpfTextView view, IClassifierAggregatorService classifierAggregatorService, IServiceProvider serviceProvider)
        {
            _view = view;
            _classifierAggregatorService = classifierAggregatorService;
            _serviceProvider = serviceProvider;

            //var mcs = ServiceProvider.GlobalProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            _service = _serviceProvider.GetService(typeof(IndentAutoConfiguratorService)) as IndentAutoConfiguratorService;
            var cmd = _service.CmdIndentSetting;
            cmd.Visible = true;

            //_layer = view.GetAdornmentLayer("IndentAutoConfigurator");

            _view.GotAggregateFocus += OnGotAggregateFocus;
            _view.LostAggregateFocus += OnLostAggregateFocus;

            UpdateSetting();
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            MessageBox.Show("MenuItemCallback");
        }

        private void OnLostAggregateFocus(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("LostAggregateFocus");
            //_service.CmdIndentSetting.Visible = false;
        }

        private void OnGotAggregateFocus(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("GotAggregateFocus");
            //_service.CmdIndentSetting.Visible = true;
            var cmd = _service.CmdIndentSetting as OleMenuCommand;
            cmd.Text = "Indent Setting: " + (
                            (CurrentCodeIndention == null) ? "(Default)"
                            : (CurrentCodeIndention.IsTab ? "Tab" : "Space (" + CurrentCodeIndention.IndentSize + ")")
                       );
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

            CurrentCodeIndention = codeIndention;
        }

        private class CodeIndention
        {
            public Boolean IsTab { get; set; }
            public Int32 IndentSize { get; set; }
        }
    }
}
