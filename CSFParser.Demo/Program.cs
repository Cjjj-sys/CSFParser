﻿using System.IO;
using System.Text;

namespace CSFParser.Demo;

class Program {
    static async Task Main(string[] args) {
        var csfFilePath = "ra2md.csf";
        var csfBytes = await File.ReadAllBytesAsync(csfFilePath);
        var header = csfBytes.Where((e, i) => i < 0x18).ToList();
        //header.ForEach(e => Console.Write($"{Char.ConvertFromUtf32(e)}·"));
        var identifier = header.GetRange(0, 4);
        Console.WriteLine(CheckIdentifier(identifier));
        PrintDebug(identifier);
    }

    static void PrintDebug(List<Byte> bytes) {
        bytes.ForEach(e => {
            var ch = Char.ConvertFromUtf32(e);
            if (ch == "" || ch == " ")
            {
                ch = "·";
            }
            Console.Write(ch);
        });
    }

    static string Bytes2Str(List<Byte> bytes) {
        StringBuilder sb = new();
        bytes.ForEach(e => {
            var ch = Char.ConvertFromUtf32(e);
            sb.Append(ch);
        });
        return sb.ToString();
    }

    static bool CheckIdentifier(List<Byte> identifier) {
        Console.WriteLine(Bytes2Str(identifier));
        if (Bytes2Str(identifier) == " FSC")
        {
            return true;
        }
        else {
            return false;
        }
    }
}