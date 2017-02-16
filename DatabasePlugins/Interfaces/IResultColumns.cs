using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace LayerGen35.DatabasePlugins
{
	/// <summary>
	/// This is a MyMeta Collection. The only two methods meant for public consumption are Count and Item.
	/// </summary>
	[GuidAttribute("72bf0df4-9eff-4f43-9d55-c6e3131da5ed"),InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IResultColumns : IList, IEnumerable
	{
		// User Meta Data
		string UserDataXPath { get; }

		/// <summary>
		/// You access items in the collect using this method. The return is the object in the collection.
		/// </summary>
		/// <param name="index">Either an integer or a string (see the remarks).
		/// </param>
		/// <remarks>
		/// The code below is using an IColumns collection, but all collections work the same way, the only difference is the return item.
		/// <list type="table">
		///		<item><term>int index</term><description>A zero based integer representing positon within the collection</description></item>
		///		<item><term>string index</term><description>A string that represents the physical name (not alias) of the item in the collection</description></item>///		
		///	</list>
		/// VBScript
		///	<code>
		/// Dim objColumn
		/// Set objColumn = objTable.Columns.Item(5)
		/// Set objColumn = objTable.Columns.Item("FirstName")
		/// 
		/// ' Loop through the collection
		/// For Each objColumn in objTable.Columns
		///	    output.writeLn objColumn.Name
		///	    output.writeLn objColumn.Alias
		///     output.writeLn objColumn.DataTypeNam
		/// Next
		///	</code>
		/// JScript
		///	<code>
		/// var objColumn;
		///	objColumn = objTable.Columns.Item(5);
		///	objColumn = objTable.Columns.Item("FirstName");
		///	
		/// for (var j = 0; j &lt; objTable.Columns.Count; j++) 
		/// {
		///	    objColumn = objTable.Columns.Item(j);
		///	    
		///	    output.writeln(objColumn.Name);
		///	    output.writeln(objColumn.Alias);
		///	    output.writeln(objColumn.DataTypeName);				
		/// }
		///	</code>
		/// </remarks>
		[DispId(0)]
		IResultColumn this[object index] { get; }

		// ICollection
		/// <summary>
		/// ICollection support. Not implemented.
		/// </summary>
		new bool IsSynchronized { get; }

		/// <summary>
		/// The number of items in the collection
		/// </summary>
		new int Count { get; }

		/// <summary>
		/// ICollection support. Not implemented.
		/// </summary>
		new void CopyTo(System.Array array, int index);

		/// <summary>
		/// ICollection support. Not implemented.
		/// </summary>
		new object SyncRoot { get; }

		// IEnumerable
		/// <summary>
		/// Used to support 'foreach' sytax. Do not call this directly.
		/// </summary>
		[DispId(-4)]
		new IEnumerator GetEnumerator();
	}
}

