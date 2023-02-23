/**
Values
Directly after the label header, the value data (string pairs) follows.
This is how it is built up:

Offset	Type	Description
0x0	char[4]	" RTS" or "WRTS"
Identifier
" RTS" means that there is no Extra Value for this label.
"WRTS" means that after the Value data, data for the Extra Value follows (see below).
Everything else is invalid.

0x4	DWORD	ValueLength
This holds the length of the Unicode string (the Value) that follows.
0x8	byte[ValueLength*2]	Value
This holds the encoded Value of the label.
Note that this is ValueLength*2 bytes long, because the value is a Unicode string, i.e. every character is a word instead of a byte.
To decode the value to a Unicode string, not every byte of the value data (or subtract it from 0xFF, see below for an example).

0x8+ValueLength*2	DWORD	
ExtraValueLength
This holds the length of the extra value string that follow.
This and the following line only exists if the identifier is "WRTS" and not " RTS".

0x8+ValueLength*2+0x4	char[ExtraValueLength]	ExtraValue
Like the label name, a non-zero-terminated string that is as long as ExtraValueLength says. If it is longer, the rest will be cut off.
Decoding the value
To decode the value to a Unicode string, not every byte of the value data (or subtract it from 0xFF).
An example in C++:

int ValueDataLength = ValueLength << 1;
for(int i = 0; i < ValueDataLength; ++i) {
  ValueData[i] = ~ValueData[i];
}
Special case of decoding the value
Although the value is a Unicode string, in Red Alert 2 & Yuri's revenge, the built-in game.fnt file mistakenly treat the Unicode code point as a Windows-1252 code point. Thus, for the characters of code point 128-159 (0x80-0x9F), the game treat these characters as Windows-1252 characters, unless the modder replace the game.fnt file with correct Unicode code points. In some mods and some editors, such characters, such as ‘’“”•, will be saved as Windows-1252 encoding. See https://i18nqa.com/debug/table-iso8859-1-vs-windows-1252.html for the difference.

When reading CSF files, it is recommended to let the user decide whether to treat these characters (0x80-0x9F) as Windows-1252 (as some mods mistakenly stored the wrong code point), or to treat these characters as Unicode (a correct font file is required). But when saving CSF, always treat these characters as the correct Unicode code point. Because in the original game.fnt file, except for Trade Mark Sign ™, other influenced characters has their correct font data in Unicode code point.

Example: a CSF file may mistakenly contains a ’ character at code point 0x92 (Windows-1252). When saving, this character should be corrected to code point 0x2019.
**/

public class CSFLabelValue {
    public int ValueLength {get;set;}
    public string Value {get;set;}
}

public class CSFLabelExtraValue : CSFLabelValue {
    public int ExtraValueLength {get;set;}
    public string ExtraValue {get;set;}
}