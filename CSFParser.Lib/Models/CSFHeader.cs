/**
    The Header
    The header of a CSF file is 0x18 bytes long.
    It is built up like this:

    Offset	Type	Description
    0x0	char[4]	" FSC"
    CSF header identifier
    If this is not " FSC", the game will not load the file.
    0x4	DWORD	CSF Version
    The version number of the CSF format.
    RA2, YR, Generals, ZH and the BFME series use version 3.
    Nox uses version 2.
    Nothing is known about the actual difference between the versions.
    Thanks to Siberian GRemlin for providing this information (see here)!
    0x8	DWORD	NumLabels
    The total amount of labels in the stringtable.
    0xC	DWORD	NumStrings
    The total amount of string pairs in the stringtable.
    (A string pair is made up of a Unicode Value and an ASCII ExtraValue, a label can contain more than one such pair, but only the first pair's Value is ever actually used by the game.)

    0x10	DWORD	(unused)
    This is not used by the game, which means it is useless.
    If you want, you can store an extra information tag there, if your program could use one (assuming you want to write a program that reads CSF files).
    0x14	DWORD	Language
    The language value for this stringtable.
    See below for a list

    Language
    The language DWORD can have the following values (others will be recognized as "Unknown"):

    0 = US (English)*
    1 = UK (English)
    2 = German*
    3 = French*
    4 = Spanish
    5 = Italian
    6 = Japanese
    7 = Jabberwockie
    8 = Korean*
    9 = Chinese*
    >9 = Unknown
    * RA2/YR has been released in this language.
**/
public class CSFHeader {
    public short Version {get;set;}
    public short NumLabels {get;set;}
    public short NumStrings {get;set;}
    public short unused {get;set;}
    public CSFLanguage Language {get;set;}
}

public enum CSFLanguage {
    US,
    UK,
    German,
    French,
    Spanish,
    Italian,
    Japanese,
    Jabberwockie,
    Korean,
    Chinese,
    Unknown
}