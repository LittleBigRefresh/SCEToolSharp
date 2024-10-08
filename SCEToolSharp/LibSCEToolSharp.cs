﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace SCEToolSharp;

[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public static unsafe partial class LibSceToolSharp
{
	[LibraryImport("scetool")]
	private static partial int libscetool_init();
	
	[LibraryImport("scetool")]
	private static partial void frontend_print_infos(byte* file);

	[LibraryImport("scetool")]
	private static partial void frontend_decrypt(byte* fileIn, byte* fileOut);

	[LibraryImport("scetool")]
	private static partial void frontend_encrypt(byte* fileIn, byte* fileOut);

	[LibraryImport("scetool")]
	private static partial void rap_set_directory(byte* dirPath);
	
	[LibraryImport("scetool")]
	private static partial void set_idps_key(byte* key);
	
	[LibraryImport("scetool")]
	private static partial void set_act_dat_file_path(byte* filePath);
	
	[LibraryImport("scetool")]
	private static partial void set_rif_file_path(byte* filePath);
	
	[LibraryImport("scetool")]
	private static partial void set_disc_encrypt_options();
	
	[LibraryImport("scetool")]
	private static partial void set_npdrm_encrypt_options();
	
	[LibraryImport("scetool")]
	private static partial void set_npdrm_content_id(byte* contentId);
	
	[LibraryImport("scetool")]
	private static partial byte* get_content_id(byte* path);

	static LibSceToolSharp()
	{
		int result = libscetool_init();
		Debug.WriteLine(nameof(libscetool_init) + " returned " + result);
	}

	public static void Decrypt(string input, string output)
	{
		fixed (byte* inputPtr = Encoding.UTF8.GetBytes(input + "\0"))
			fixed (byte* outputPtr = Encoding.UTF8.GetBytes(output + "\0"))
				frontend_decrypt(inputPtr, outputPtr);
	}
	
	public static void Encrypt(string input, string output)
	{
		fixed (byte* inputPtr = Encoding.UTF8.GetBytes(input + "\0"))
			fixed (byte* outputPtr = Encoding.UTF8.GetBytes(output + "\0"))
				frontend_encrypt(inputPtr, outputPtr);
	}
	
	public static void PrintInfos(string file)
	{
		fixed (byte* filePtr = Encoding.UTF8.GetBytes(file + "\0"))
			frontend_print_infos(filePtr);
	}
	
	public static void SetNpdrmContentId(string contentId)
	{
		fixed (byte* contentIdPtr = Encoding.UTF8.GetBytes(contentId + "\0"))
			set_npdrm_content_id(contentIdPtr);
	}

	public static void SetRapDirectory(string dirPath)
	{
		if (!Directory.Exists(dirPath)) throw new DirectoryNotFoundException(dirPath);
		
		fixed (byte* dirPtr = Encoding.UTF8.GetBytes(dirPath + '\0'))
			rap_set_directory(dirPtr);
	}
		
	public static void SetIdpsKey(byte[] key)
	{
		fixed (byte* keyPtr = key)
			set_idps_key(keyPtr);
	}
	
	public static void SetActDatFilePath(string filePath)
	{
		if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);
		
		fixed (byte* dirPtr = Encoding.UTF8.GetBytes(filePath + '\0'))
			set_act_dat_file_path(dirPtr);
	}
	
	public static void SetRifPath(string filePath)
	{
		if (!Directory.Exists(filePath)) throw new DirectoryNotFoundException(filePath);
		
		fixed (byte* dirPtr = Encoding.UTF8.GetBytes(filePath + '\0'))
			set_rif_file_path(dirPtr);
	}
	
	public static string? GetContentId(string path)
	{
		if (!File.Exists(path)) throw new FileNotFoundException(path);

		fixed (byte* pathPtr = Encoding.UTF8.GetBytes(path + "\0"))
		{
			byte* data = get_content_id(pathPtr);

			if (data == null)
			{
				return null;
			}
			
			const int contentIdSize = 0x30;
			
			return Encoding.UTF8.GetString(new Span<byte>(data, contentIdSize));
		}
	}

	public static void SetDiscEncryptOptions() => set_disc_encrypt_options();
	public static void SetNpdrmEncryptOptions() => set_npdrm_encrypt_options();
	public static int Init() => libscetool_init();
}
