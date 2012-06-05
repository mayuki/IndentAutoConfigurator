using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misuzilla.IndentAutoConfigurator
{
    public class IndentAutoConfiguratorService
    {
        private IndentAutoConfiguratorPackage _package;
        public IndentAutoConfiguratorService(IndentAutoConfiguratorPackage package)
        {
            _package = package;
        }

        public MenuCommand CmdIndentSetting { get { return _package.CmdIndentSetting; } }
    }
}
