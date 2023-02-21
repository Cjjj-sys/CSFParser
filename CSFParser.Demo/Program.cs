using System.IO;
using System.Text;

namespace CSFParser.Demo;

class Program {
    static async Task Main(string[] args) {
        var csfFilePath = "ra2md.csf";
        var csfBytes = (await File.ReadAllBytesAsync(csfFilePath));
        // var csfHeader = csfBytes.Where((e, i) => i < 0x18).ToList();
        // //header.ForEach(e => Console.Write($"{Char.ConvertFromUtf32(e)}·"));
        // var identifier = csfHeader.GetRange(0, 4);
        // Console.WriteLine(CheckIdentifier(identifier));
        // PrintDebug(identifier);
        // var body = csfBytes.Where((e, i) => i >= 0x18).ToList();
        CSFHeader header = new();
        List<CSFLabel> labels = new();
        State state = State.Header;
        int i = 0;
        bool isDone = false;
        while (!isDone){
            switch (state)
            {
                case State.Header:
                    var p = i;
                    var identifier = csfBytes[p..4];
                    if (CheckIdentifier(identifier))
                    {
                        p += 4;
                        var version = csfBytes[p..(p+4)];
                        header.Version = Convert.ToInt16(version);

                        p += 4;
                        var numLabels = csfBytes[p..(p+4)];
                        header.NumLabels = Convert.ToInt16(numLabels);

                        p += 4;
                        var numStrings = csfBytes[p..(p+4)];
                        header.NumStrings = Convert.ToInt16(numStrings);

                        p += 4;
                        var unused = csfBytes[p..(p+4)];
                        header.unused = Convert.ToInt16(unused);

                        p += 4;
                        var language = csfBytes[p..(p+4)];
                        header.Language = (CSFLanguage)Convert.ToInt16(language);

                        i = p;
                        state = State.Labels;
                    }
                    else {
                        throw new NotSupportedException("不支持此 csf 文件");
                    }
                    break;
                case State.Labels:

                    isDone = true;
                    break;
            }
        }
    }

    enum State
    {
        Header,
        Labels
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

    static string Bytes2Str(Byte[] bytes) {
        StringBuilder sb = new();
        bytes.ToList().ForEach(e => {
            var ch = Char.ConvertFromUtf32(e);
            sb.Append(ch);
        });
        return sb.ToString();
    }

    static bool CheckIdentifier(List<Byte> identifier) {
        if (Bytes2Str(identifier) == " FSC")
        {
            return true;
        }
        else {
            return false;
        }
    }

    static bool CheckIdentifier(Byte[] identifier) {
        if (Bytes2Str(identifier) == " FSC")
        {
            return true;
        }
        else {
            return false;
        }
    }
}