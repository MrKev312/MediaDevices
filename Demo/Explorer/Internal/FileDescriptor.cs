﻿using System;

namespace ExplorerCtrl.Internal;

/// <summary>
/// Class representing a virtual file for use by drag/drop or the clipboard.
/// </summary>
public class FileDescriptor
{
	/// <summary>
	/// Gets or sets the name of the file.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the (optional) length of the file.
	/// </summary>
	public long? Length { get; set; }

	/// <summary>
	/// Gets or sets the (optional) change time of the file.
	/// </summary>
	public DateTime? ChangeTimeUtc { get; set; }

	public IExplorerItem Item { get; set; }

	public string DevName { get; set; }

	public bool IsDirectory { get; set; }
}
