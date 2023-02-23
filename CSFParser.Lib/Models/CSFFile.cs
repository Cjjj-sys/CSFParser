public class CSFFile {
    public CSFHeader Header {get;set;}
    public List<CSFLabel> Labels {get;set;}
}

public enum CSFIdentifier {
    FSC,
    LBL,
    RTS,
    WRTS
}