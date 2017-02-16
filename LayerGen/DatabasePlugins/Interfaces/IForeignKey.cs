using System;
using System.Runtime.InteropServices;

namespace LayerGen.DatabasePlugins
{
	/// <summary>
	/// IForeignKey represents an foreign key on a table in your DBMS.
	/// </summary>
	/// <remarks>
	///	IForeignKey Collections:
	/// <list type="table">
	///		<item><term>ForeignColumns</term><description>A collection of columns that are in the foreign table that make up the key</description></item>
	///		<item><term>PrimaryColumns</term><description>A collection of columns that are in the parent table that make up the key</description></item>
	///		<item><term>Properties</term><description>A collection that can hold key/value pairs of your choosing</description></item>	
	///		<item><term>GlobalProperties</term><description>A collection that can hold key/value pairs of your choosing for all ForeignKeys with the same Database</description></item>
	///		<item><term>AllProperties</term><description>A collection that combines the Properties and GlobalProperties Collections</description></item>
	///	</list>
	/// </remarks>
	/// <example>
	/// VBScript
	/// <code>
	/// Dim objForeignKey
	/// For Each objForeignKey in objTable.ForeignKeys
	///     output.writeLn objForeignKey.Name
	///	    output.writeLn objForeignKey.Alias
	/// Next
	/// </code>
	/// JScript
	/// <code>
	/// var objForeignKey;
	/// for (var j = 0; j &lt; objTable.ForeignKeys; j++) 
	/// {
	///	    objColumn = objTable.ForeignKeys.Item(j);
	///	    
	///	    output.writeln(objForeignKey.Name);
	///	    output.writeln(objForeignKey.Alias);
	/// }
	/// </code>
	/// </example>
	[GuidAttribute("c4c07016-a7a5-4415-b837-285a2b4f9ec1"),InterfaceType(ComInterfaceType.InterfaceIsDual)]	 
	public interface IForeignKey
	{
		/// <summary>
		/// You can override the physical name of the foreign key. If you do not provide an Alias the value of 'ForeignKey.Name' is returned.
		/// If your foreign key in your DBMS is 'TXT_FIRST_NAME' you might want to give it an Alias of 'FirstName' so that your business object property will be a nice name.
		/// You can provide an Alias the User Meta Data window. You can also set this during a script and then call MyMeta.SaveUserMetaData().
		/// See <see cref="Name"/>
		/// </summary>
		[DispId(0)]
		string Alias { get; set; }
	
		/// <summary>
		/// This is the physical name of the primary key name as stored in your DBMS system. See <see cref="Alias"/>
		/// </summary>
		string Name { get; }

		// Objects
		/// <summary>
		/// The parent key table of this foreign key
		/// </summary>
		ITable PrimaryTable { get; }

		/// <summary>
		/// The foreign key table of this foreign key
		/// </summary>
		ITable ForeignTable { get; }

		// Collections
		/// <summary>
		/// A collection of columns that are in the foreign table (not in this table) that make up the key
		/// </summary>
		IColumns ForeignColumns { get; }

		/// <summary>
		/// A collection of columns that are in the parent table that make up the key
		/// </summary>
		IColumns PrimaryColumns { get; }

		/// <summary>
		/// The Properties for this ForeignKey. These are user defined and are typically stored in 'UserMetaData.xml' unless changed in the Default Settings dialog.
		/// Properties consist of key/value pairs.  You can populate this collection during your script or via the Dockable window. 
		/// To save any data added to this collection call MyMeta.SaveUserMetaData(). See <see cref="IProperty"/>
		/// </summary>
		IPropertyCollection Properties { get; }

		/// <summary>
		/// The Properties for all ForeignKeys within the same Database. These are user defined and are typically stored in 'UserMetaData.xml' unless changed in the Default Settings dialog.
		/// Properties consist of key/value pairs.  You can populate this collection during your script or via the Dockable window. 
		/// To save any data added to this collection call MyMeta.SaveUserMetaData(). See <see cref="IProperty"/>
		/// </summary>
		IPropertyCollection GlobalProperties { get; }

		/// <summary>
		/// AllProperties is essentially a read-only collection consisting of a combination of both the <see cref="Properties"/> (local) collection and the <see cref="GlobalProperties"/> (global) collection. The local properties are added first, 
		/// and then the global properties are added however, only global properties for which no local property exists -- are added. This makes local properties overlay global properties. Global properties can
		/// act as a default value which can be overridden by a local property. See <see cref="IProperty"/>.
		/// </summary>
		IPropertyCollection AllProperties { get; }

		// User Meta Data
		string UserDataXPath { get; }

		// Properties
		/// <summary>
		/// The type of the foreign key. One of the following (or blank):
		/// <list type="table">
		///		<item><term>CASCADE</term><description>A referential action of CASCADE was specified</description></item>
		///		<item><term>SET NULL</term><description>A referential action of SET NULL was specified</description></item>		
		///		<item><term>SET DEFAULT</term><description>A referential action of SET DEFAULT was specified</description></item>
		///		<item><term>NO ACTION</term><description>A referential action of NO ACTION was specified</description></item>	
		///	</list>
		/// </summary>
		string UpdateRule { get; }

		/// <summary>
		/// The type of the foreign key. One of the following (or blank):
		/// <list type="table">
		///		<item><term>CASCADE</term><description>A referential action of CASCADE was specified</description></item>
		///		<item><term>SET NULL</term><description>A referential action of SET NULL was specified</description></item>		
		///		<item><term>SET DEFAULT</term><description>A referential action of SET DEFAULT was specified</description></item>
		///		<item><term>NO ACTION</term><description>A referential action of NO ACTION was specified</description></item>	
		///	</list>
		/// </summary>
		string DeleteRule { get; }

		/// <summary>
		/// The Primary Key name
		/// </summary>
		string PrimaryKeyName { get; }

		/// <summary>
		/// The type of the foreign key. One of the following (or blank):
		/// <list type="table">
		///		<item><term>INITIALLY_DEFERRED</term><description></description></item>
		///		<item><term>INITIALLY_IMMEDIATE	</term><description></description></item>		
		///		<item><term>NOT_DEFERRABLE</term><description></description></item>
		///		<item><term>UNKNOWN</term><description></description></item>
		///	</list>
		/// </summary>
        string Deferrability { get; }

        /// <summary>
        /// Fetch any database specific meta data through this generic interface by key. The keys will have to be defined by the specific database provider
        /// </summary>
        /// <param name="key">A key identifying the type of meta data desired.</param>
        /// <returns>A meta-data object or collection.</returns>
        object DatabaseSpecificMetaData(string key);
	}
}

