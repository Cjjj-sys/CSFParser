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
                    var identifier = csfBytes[i..4];
                    if (GetIdentifier(identifier) == CSFIdentifier.FSC)
                    {
                        i += 4;
                        var version = csfBytes[i..(i+4)];
                        header.Version = BitConverter.ToInt32(version);

                        i += 4;
                        var numLabels = csfBytes[i..(i+4)];
                        header.NumLabels = BitConverter.ToInt32(numLabels);

                        i += 4;
                        var numStrings = csfBytes[i..(i+4)];
                        header.NumStrings = BitConverter.ToInt32(numStrings);

                        i += 4;
                        var unused = csfBytes[i..(i+4)];
                        header.unused = BitConverter.ToInt32(unused);

                        i += 4;
                        var language = csfBytes[i..(i+4)];
                        header.Language = (CSFLanguage)BitConverter.ToInt32(language);

                        state = State.Labels;
                    }
                    else {
                        throw new NotSupportedException("不支持此 csf 文件");
                    }
                    break;
                case State.Labels:
                    var labelIdentifier = csfBytes[i..(i+4)];
                    if (GetIdentifier(labelIdentifier) == CSFIdentifier.LBL)
                    {
                        CSFLabel label = new();

                        i += 4;
                        var numberOfStringPairs = csfBytes[i..(i+4)];
                        label.NumberOfStringPairs = BitConverter.ToInt32(numberOfStringPairs);

                        i += 4;
                        var LabelNameLength = csfBytes[i..(i+4)];
                        label.LabelNameLength = BitConverter.ToInt32(LabelNameLength);

                        i += 4;
                        var labelName = csfBytes[i..(i+label.LabelNameLength)];
                        label.LabelName = Bytes2Str(labelName);

                        i += label.LabelNameLength;
                        var valueIdentifier = csfBytes[i..(i+4)];

                        if (GetIdentifier(valueIdentifier) == CSFIdentifier.RTS)
                        {
                            CSFLabelValue value = new();
                        }
                        else if (GetIdentifier(valueIdentifier) == CSFIdentifier.WRTS) {
                            CSFLabelExtraValue extraValue = new();
                        }
                    }

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

    static CSFIdentifier GetIdentifier(Byte[] identifier) => Bytes2Str(identifier) switch 
    {
        " FSC" => CSFIdentifier.FSC,
        " LBL" => CSFIdentifier.LBL,
        " RTS" => CSFIdentifier.RTS,
        "WRTS" => CSFIdentifier.WRTS,
        _ => throw new NotSupportedException($"不受支持的 identifier: {Bytes2Str(identifier)}")
    };
}