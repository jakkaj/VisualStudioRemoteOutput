//------------------------------------------------------------------------------
// <copyright file="VisualStudioRemoteOutputPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace VisualStudioRemoteOutputPlugin
{
    [Guid("eaeeba03-16e7-428f-85c4-4fdfbe48c4b3")]
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\14.0")]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("VisualStudioRemoteOutput", "Remote output for build and debug windows", "1.0")]
    public class VisualStudioRemoteOutputPackage : Package
    {
    }
}
