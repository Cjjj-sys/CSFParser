
/**
    Label header
    The label data begins with a label header, which is built up like this:

    Offset	Type	Description
    0x0	char[4]	" LBL"
    Label identifier
    If this is not " LBL", the game will not recognize the following data as label data and read the next 4 bytes.
    0x4	DWORD	Number of string pairs
    This is the number of string pairs associated with this label. Usual value is 1.
    0x8	DWORD	LabelNameLength
    This value holds the size of the label name that follows.
    0xC	char[LabelNameLength]	LabelName
    A non-zero-terminated string that is as long as the DWORD at 0x8 says. If it is longer, the rest will be cut off.
    The first label in ra2md.csf can be found at 0x18.
    Note: Spaces, tabs and line breaks will be formatted out of the label's name, therefore they cannot be used. However, although spaces will be formatted out, there is a label named "gui:password entry box label" in the ra2.csf and ra2md.csf file that comes with the game.
    Note: The label name is case-insensitive. If a label name is shown up for more than once, the last item will actually be loaded by the game.
**/
public class CSFLabel {
    public int NumberOfStringPairs {get;set;}
    public int LabelNameLength {get;set;}
    public string LabelName {get;set;}

    public CSFLabelValue Value {get;set;}
}