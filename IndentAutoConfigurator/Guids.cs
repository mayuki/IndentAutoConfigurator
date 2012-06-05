// Guids.cs
// MUST match guids.h
using System;

namespace Misuzilla.IndentAutoConfigurator
{
    static class GuidList
    {
        public const string guidIndentAutoConfiguratorPkgString = "e6ef0bd4-6fcd-444e-bb9f-02461df042c5";
        public const string guidIndentAutoConfiguratorCmdSetString = "48c2f78d-423e-4af6-b5e7-a22ff29d7e4d";

        public static readonly Guid guidIndentAutoConfiguratorCmdSet = new Guid(guidIndentAutoConfiguratorCmdSetString);
    };
}