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
        List<CSFLabel> csfLabels = new();
        State state = State.Header;
        int i = 0;
        int currentNumLabels = 0;
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

                        i += 4;
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
                        CSFLabel csfLabel = new();

                        i += 4;
                        var numberOfStringPairs = csfBytes[i..(i+4)];
                        csfLabel.NumberOfStringPairs = BitConverter.ToInt32(numberOfStringPairs);

                        i += 4;
                        var LabelNameLength = csfBytes[i..(i+4)];
                        csfLabel.LabelNameLength = BitConverter.ToInt32(LabelNameLength);

                        i += 4;
                        var labelName = csfBytes[i..(i+csfLabel.LabelNameLength)];
                        csfLabel.LabelName = Bytes2Str(labelName);

                        i += csfLabel.LabelNameLength;
                        var valueIdentifier = csfBytes[i..(i+4)];

                        if (GetIdentifier(valueIdentifier) == CSFIdentifier.RTS)
                        {
                            CSFLabelValue csfValue = new();

                            i += 4;
                            var valueLength = csfBytes[i..(i+4)];
                            csfValue.ValueLength = BitConverter.ToInt32(valueLength);

                            i += 4;
                            var value = csfBytes[i..(i+csfValue.ValueLength*2)];
                            csfValue.Value = Unicode2Str(DecodeValue(value, csfValue.ValueLength));

                            i += csfValue.ValueLength*2;
                            csfLabel.Value = csfValue;
                            
                        }
                        else if (GetIdentifier(valueIdentifier) == CSFIdentifier.WRTS) {
                            CSFLabelExtraValue csfExtraValue = new();
                            
                            i += 4;
                            var valueLength = csfBytes[i..(i+4)];
                            csfExtraValue.ValueLength = BitConverter.ToInt32(valueLength);

                            i += 4;
                            var value = csfBytes[i..(i+(csfExtraValue.ValueLength*2))];
                            csfExtraValue.Value = Unicode2Str(DecodeValue(value, csfExtraValue.ValueLength));

                            i += (csfExtraValue.ValueLength*2);
                            var extraValueLength = csfBytes[i..(i+4)];
                            csfExtraValue.ExtraValueLength = BitConverter.ToInt32(extraValueLength);

                            i += 4;
                            var extraValue = csfBytes[i..(i+(csfExtraValue.ExtraValueLength))];
                            csfExtraValue.ExtraValue = Bytes2Str(extraValue);
                            
                            i += (csfExtraValue.ExtraValueLength);
                            csfLabel.Value = csfExtraValue;
                        }
                        csfLabels.Add(csfLabel);
                    }
                    currentNumLabels += 1;
                    if (currentNumLabels >= header.NumLabels)
                    {
                        isDone = true;
                    }
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

    static string Bytes2Str(Byte[] bytes) {
        StringBuilder sb = new();
        bytes.ToList().ForEach(e => {
            var ch = Char.ConvertFromUtf32(e);
            sb.Append(ch);
        });
        return sb.ToString();
    }

    static Byte[] DecodeValue(Byte[] valueData, int valueLength) {
        int ValueDataLength = valueLength << 1;
        for(int i = 0; i < ValueDataLength; ++i) {
            valueData[i] = (byte)~valueData[i];
        }
        return valueData;
    }

    static string Unicode2Str(Byte[] bytes) {
        UnicodeEncoding unicode = new();
        return unicode.GetString(bytes);
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