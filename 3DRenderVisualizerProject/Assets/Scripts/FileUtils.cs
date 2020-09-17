using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public static class FileUtils
{
	public static FileInfo[] GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
	{
		if (extensions == null)
			return null;
		IEnumerable<FileInfo> files = dir.EnumerateFiles(); //EnumerateFiles needs .NET 4 (you can use GetFiles instead)
		return files.Where(f => extensions.Contains(f.Extension, StringComparer.OrdinalIgnoreCase)).ToArray();
	}
}

