// Copyright � 2004, 2015, Oracle and/or its affiliates.  All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Security;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyDescription("ADO.Net driver for MySQL")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Oracle")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("Copyright � 2004, 2015, Oracle and/or its affiliates. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]
#if !CF
[assembly: AssemblyTitle("MySql.Data.dll")]
#if !RT
[assembly: AllowPartiallyTrustedCallers()]
#endif
#else
[assembly: AssemblyTitle("MySql.Data.CF.dll")]
#endif

#if NET_40_OR_GREATER
[assembly: SecurityRules(SecurityRuleSet.Level1)]
#endif


#if CF
[assembly: AssemblyFlags(AssemblyNameFlags.Retargetable)]
#endif