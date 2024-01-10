using System.Collections.ObjectModel;

namespace CodeDesignPlus.Net.File.Storage.Abstractions;

/// <summary>
///  https://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types
/// </summary>
public class ApacheMime
{
    public string Extension { get; set; }
    public string Name { get; set; }
    public string MimeType { get; set; }

    public static ReadOnlyCollection<ApacheMime> ApacheMimes
    {
        get
        {
            return new ReadOnlyCollection<ApacheMime>(apacheMimes);
        }
    }

    /// <summary>
    /// Lista Mime Types https://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types
    /// </summary>
    private static readonly List<ApacheMime> apacheMimes = new ()
        {
            new ApacheMime()
            {
                Extension = ".x3d",
                Name = "3D Crossword Plugin",
                MimeType = "application/vnd.hzn-3d-crossword",
            },
            new ApacheMime()
            {
                Extension = ".3gp",
                Name = "3GP",
                MimeType = "video/3gpp",
            },
            new ApacheMime()
            {
                Extension = ".3g2",
                Name = "3GP2",
                MimeType = "video/3gpp2",
            },
            new ApacheMime()
            {
                Extension = ".mseq",
                Name = "3GPP MSEQ File",
                MimeType = "application/vnd.mseq",
            },
            new ApacheMime()
            {
                Extension = ".pwn",
                Name = "3M Post It Notes",
                MimeType = "application/vnd.3m.post-it-notes",
            },
            new ApacheMime()
            {
                Extension = ".plb",
                Name = "3rd Generation Partnership Project – Pic Large",
                MimeType = "application/vnd.3gpp.pic-bw-large",
            },
            new ApacheMime()
            {
                Extension = ".psb",
                Name = "3rd Generation Partnership Project – Pic Small",
                MimeType = "application/vnd.3gpp.pic-bw-small",
            },
            new ApacheMime()
            {
                Extension = ".pvb",
                Name = "3rd Generation Partnership Project – Pic Var",
                MimeType = "application/vnd.3gpp.pic-bw-var",
            },
            new ApacheMime()
            {
                Extension = ".tcap",
                Name = "3rd Generation Partnership Project – Transaction Capabilities Application Part",
                MimeType = "application/vnd.3gpp2.tcap",
            },
            new ApacheMime()
            {
                Extension = ".7z",
                Name = "7-Zip",
                MimeType = "application/x-7z-compressed",
            },
            new ApacheMime()
            {
                Extension = ".abw",
                Name = "AbiWord",
                MimeType = "application/x-abiword",
            },
            new ApacheMime()
            {
                Extension = ".ace",
                Name = "Ace Archive",
                MimeType = "application/x-ace-compressed",
            },
            new ApacheMime()
            {
                Extension = ".acc",
                Name = "Active Content Compression",
                MimeType = "application/vnd.americandynamics.acc",
            },
            new ApacheMime()
            {
                Extension = ".acu",
                Name = "ACU Cobol",
                MimeType = "application/vnd.acucobol",
            },
            new ApacheMime()
            {
                Extension = ".atc",
                Name = "ACU Cobol",
                MimeType = "application/vnd.acucorp",
            },
            new ApacheMime()
            {
                Extension = ".adp",
                Name = "Adaptive differential pulse-code modulation",
                MimeType = "audio/adpcm",
            },
            new ApacheMime()
            {
                Extension = ".aab",
                Name = "Adobe (Macropedia) Authorware – Binary File",
                MimeType = "application/x-authorware-bin",
            },
            new ApacheMime()
            {
                Extension = ".aam",
                Name = "Adobe (Macropedia) Authorware – Map",
                MimeType = "application/x-authorware-map",
            },
            new ApacheMime()
            {
                Extension = ".aas",
                Name = "Adobe (Macropedia) Authorware – Segment File",
                MimeType = "application/x-authorware-seg",
            },
            new ApacheMime()
            {
                Extension = ".air",
                Name = "Adobe AIR Application",
                MimeType = "application/vnd.adobe.air-application-installer-package+zip",
            },
            new ApacheMime()
            {
                Extension = ".swf",
                Name = "Adobe Flash",
                MimeType = "application/x-shockwave-flash",
            },
            new ApacheMime()
            {
                Extension = ".fxp",
                Name = "Adobe Flex Project",
                MimeType = "application/vnd.adobe.fxp",
            },
            new ApacheMime()
            {
                Extension = ".pdf",
                Name = "Adobe Portable Document Format",
                MimeType = "application/pdf",
            },
            new ApacheMime()
            {
                Extension = ".ppd",
                Name = "Adobe PostScript Printer Description File Format",
                MimeType = "application/vnd.cups-ppd",
            },
            new ApacheMime()
            {
                Extension = ".dir",
                Name = "Adobe Shockwave Player",
                MimeType = "application/x-director",
            },
            new ApacheMime()
            {
                Extension = ".xdp",
                Name = "Adobe XML Data Package",
                MimeType = "application/vnd.adobe.xdp+xml",
            },
            new ApacheMime()
            {
                Extension = ".xfdf",
                Name = "Adobe XML Forms Data Format",
                MimeType = "application/vnd.adobe.xfdf",
            },
            new ApacheMime()
            {
                Extension = ".aac",
                Name = "Advanced Audio Coding (AAC)",
                MimeType = "audio/x-aac",
            },
            new ApacheMime()
            {
                Extension = ".ahead",
                Name = "Ahead AIR Application",
                MimeType = "application/vnd.ahead.space",
            },
            new ApacheMime()
            {
                Extension = ".azf",
                Name = "AirZip FileSECURE",
                MimeType = "application/vnd.airzip.filesecure.azf",
            },
            new ApacheMime()
            {
                Extension = ".azs",
                Name = "AirZip FileSECURE",
                MimeType = "application/vnd.airzip.filesecure.azs",
            },
            new ApacheMime()
            {
                Extension = ".azw",
                Name = "Amazon Kindle eBook format",
                MimeType = "application/vnd.amazon.ebook",
            },
            new ApacheMime()
            {
                Extension = ".ami",
                Name = "AmigaDE",
                MimeType = "application/vnd.amiga.ami",
            },
            new ApacheMime()
            {
                Extension = "N/A",
                Name = "Andrew Toolkit",
                MimeType = "application/andrew-inset",
            },
            new ApacheMime()
            {
                Extension = ".apk",
                Name = "Android Package Archive",
                MimeType = "application/vnd.android.package-archive",
            },
            new ApacheMime()
            {
                Extension = ".cii",
                Name = "ANSER-WEB Terminal Client – Certificate Issue",
                MimeType = "application/vnd.anser-web-certificate-issue-initiation",
            },
            new ApacheMime()
            {
                Extension = ".fti",
                Name = "ANSER-WEB Terminal Client – Web Funds Transfer",
                MimeType = "application/vnd.anser-web-funds-transfer-initiation",
            },
            new ApacheMime()
            {
                Extension = ".atx",
                Name = "Antix Game Player",
                MimeType = "application/vnd.antix.game-component",
            },
            new ApacheMime()
            {
                Extension = ".mpkg",
                Name = "Apple Installer Package",
                MimeType = "application/vnd.apple.installer+xml",
            },
            new ApacheMime()
            {
                Extension = ".aw",
                Name = "Applixware",
                MimeType = "application/applixware",
            },
            new ApacheMime()
            {
                Extension = ".les",
                Name = "Archipelago Lesson Player",
                MimeType = "application/vnd.hhe.lesson-player",
            },
            new ApacheMime()
            {
                Extension = ".swi",
                Name = "Arista Networks Software Image",
                MimeType = "application/vnd.aristanetworks.swi",
            },
            new ApacheMime()
            {
                Extension = ".s",
                Name = "Assembler Source File",
                MimeType = "text/x-asm",
            },
            new ApacheMime()
            {
                Extension = ".atomcat",
                Name = "Atom Publishing Protocol",
                MimeType = "application/atomcat+xml",
            },
            new ApacheMime()
            {
                Extension = ".atomsvc",
                Name = "Atom Publishing Protocol Service Document",
                MimeType = "application/atomsvc+xml",
            },
            new ApacheMime()
            {
                Extension = ".atom, .xml",
                Name = "Atom Syndication Format",
                MimeType = "application/atom+xml",
            },
            new ApacheMime()
            {
                Extension = ".ac",
                Name = "Attribute Certificate",
                MimeType = "application/pkix-attr-cert",
            },
            new ApacheMime()
            {
                Extension = ".aif",
                Name = "Audio Interchange File Format",
                MimeType = "audio/x-aiff",
            },
            new ApacheMime()
            {
                Extension = ".avi",
                Name = "Audio Video Interleave (AVI)",
                MimeType = "video/x-msvideo",
            },
            new ApacheMime()
            {
                Extension = ".aep",
                Name = "Audiograph",
                MimeType = "application/vnd.audiograph",
            },
            new ApacheMime()
            {
                Extension = ".dxf",
                Name = "AutoCAD DXF",
                MimeType = "image/vnd.dxf",
            },
            new ApacheMime()
            {
                Extension = ".dwf",
                Name = "Autodesk Design Web Format (DWF)",
                MimeType = "model/vnd.dwf",
            },
            new ApacheMime()
            {
                Extension = ".par",
                Name = "BAS Partitur Format",
                MimeType = "text/plain-bas",
            },
            new ApacheMime()
            {
                Extension = ".bcpio",
                Name = "Binary CPIO Archive",
                MimeType = "application/x-bcpio",
            },
            new ApacheMime()
            {
                Extension = ".bin",
                Name = "Binary Data",
                MimeType = "application/octet-stream",
            },
            new ApacheMime()
            {
                Extension = ".bmp",
                Name = "Bitmap Image File",
                MimeType = "image/bmp",
            },
            new ApacheMime()
            {
                Extension = ".torrent",
                Name = "BitTorrent",
                MimeType = "application/x-bittorrent",
            },
            new ApacheMime()
            {
                Extension = ".cod",
                Name = "Blackberry COD File",
                MimeType = "application/vnd.rim.cod",
            },
            new ApacheMime()
            {
                Extension = ".mpm",
                Name = "Blueice Research Multipass",
                MimeType = "application/vnd.blueice.multipass",
            },
            new ApacheMime()
            {
                Extension = ".bmi",
                Name = "BMI Drawing Data Interchange",
                MimeType = "application/vnd.bmi",
            },
            new ApacheMime()
            {
                Extension = ".sh",
                Name = "Bourne Shell Script",
                MimeType = "application/x-sh",
            },
            new ApacheMime()
            {
                Extension = ".btif",
                Name = "BTIF",
                MimeType = "image/prs.btif",
            },
            new ApacheMime()
            {
                Extension = ".rep",
                Name = "BusinessObjects",
                MimeType = "application/vnd.businessobjects",
            },
            new ApacheMime()
            {
                Extension = ".bz",
                Name = "Bzip Archive",
                MimeType = "application/x-bzip",
            },
            new ApacheMime()
            {
                Extension = ".bz2",
                Name = "Bzip2 Archive",
                MimeType = "application/x-bzip2",
            },
            new ApacheMime()
            {
                Extension = ".csh",
                Name = "C Shell Script",
                MimeType = "application/x-csh",
            },
            new ApacheMime()
            {
                Extension = ".c",
                Name = "C Source File",
                MimeType = "text/x-c",
            },
            new ApacheMime()
            {
                Extension = ".cdxml",
                Name = "CambridgeSoft Chem Draw",
                MimeType = "application/vnd.chemdraw+xml",
            },
            new ApacheMime()
            {
                Extension = ".css",
                Name = "Cascading Style Sheets (CSS)",
                MimeType = "text/css",
            },
            new ApacheMime()
            {
                Extension = ".cdx",
                Name = "ChemDraw eXchange file",
                MimeType = "chemical/x-cdx",
            },
            new ApacheMime()
            {
                Extension = ".cml",
                Name = "Chemical Markup Language",
                MimeType = "chemical/x-cml",
            },
            new ApacheMime()
            {
                Extension = ".csml",
                Name = "Chemical Style Markup Language",
                MimeType = "chemical/x-csml",
            },
            new ApacheMime()
            {
                Extension = ".cdbcmsg",
                Name = "CIM Database",
                MimeType = "application/vnd.contact.cmsg",
            },
            new ApacheMime()
            {
                Extension = ".cla",
                Name = "Claymore Data Files",
                MimeType = "application/vnd.claymore",
            },
            new ApacheMime()
            {
                Extension = ".c4g",
                Name = "Clonk Game",
                MimeType = "application/vnd.clonk.c4group",
            },
            new ApacheMime()
            {
                Extension = ".sub",
                Name = "Close Captioning – Subtitle",
                MimeType = "image/vnd.dvb.subtitle",
            },
            new ApacheMime()
            {
                Extension = ".cdmia",
                Name = "Cloud Data Management Interface (CDMI) – Capability",
                MimeType = "application/cdmi-capability",
            },
            new ApacheMime()
            {
                Extension = ".cdmic",
                Name = "Cloud Data Management Interface (CDMI) – Contaimer",
                MimeType = "application/cdmi-container",
            },
            new ApacheMime()
            {
                Extension = ".cdmid",
                Name = "Cloud Data Management Interface (CDMI) – Domain",
                MimeType = "application/cdmi-domain",
            },
            new ApacheMime()
            {
                Extension = ".cdmio",
                Name = "Cloud Data Management Interface (CDMI) – Object",
                MimeType = "application/cdmi-object",
            },
            new ApacheMime()
            {
                Extension = ".cdmiq",
                Name = "Cloud Data Management Interface (CDMI) – Queue",
                MimeType = "application/cdmi-queue",
            },
            new ApacheMime()
            {
                Extension = ".c11amc",
                Name = "ClueTrust CartoMobile – Config",
                MimeType = "application/vnd.cluetrust.cartomobile-config",
            },
            new ApacheMime()
            {
                Extension = ".c11amz",
                Name = "ClueTrust CartoMobile – Config Package",
                MimeType = "application/vnd.cluetrust.cartomobile-config-pkg",
            },
            new ApacheMime()
            {
                Extension = ".ras",
                Name = "CMU Image",
                MimeType = "image/x-cmu-raster",
            },
            new ApacheMime()
            {
                Extension = ".dae",
                Name = "COLLADA",
                MimeType = "model/vnd.collada+xml",
            },
            new ApacheMime()
            {
                Extension = ".csv",
                Name = "Comma-Seperated Values",
                MimeType = "text/csv",
            },
            new ApacheMime()
            {
                Extension = ".cpt",
                Name = "Compact Pro",
                MimeType = "application/mac-compactpro",
            },
            new ApacheMime()
            {
                Extension = ".wmlc",
                Name = "Compiled Wireless Markup Language (WMLC)",
                MimeType = "application/vnd.wap.wmlc",
            },
            new ApacheMime()
            {
                Extension = ".cgm",
                Name = "Computer Graphics Metafile",
                MimeType = "image/cgm",
            },
            new ApacheMime()
            {
                Extension = ".ice",
                Name = "CoolTalk",
                MimeType = "x-conference/x-cooltalk",
            },
            new ApacheMime()
            {
                Extension = ".cmx",
                Name = "Corel Metafile Exchange (CMX)",
                MimeType = "image/x-cmx",
            },
            new ApacheMime()
            {
                Extension = ".xar",
                Name = "CorelXARA",
                MimeType = "application/vnd.xara",
            },
            new ApacheMime()
            {
                Extension = ".cmc",
                Name = "CosmoCaller",
                MimeType = "application/vnd.cosmocaller",
            },
            new ApacheMime()
            {
                Extension = ".cpio",
                Name = "CPIO Archive",
                MimeType = "application/x-cpio",
            },
            new ApacheMime()
            {
                Extension = ".clkx",
                Name = "CrickSoftware – Clicker",
                MimeType = "application/vnd.crick.clicker",
            },
            new ApacheMime()
            {
                Extension = ".clkk",
                Name = "CrickSoftware – Clicker – Keyboard",
                MimeType = "application/vnd.crick.clicker.keyboard",
            },
            new ApacheMime()
            {
                Extension = ".clkp",
                Name = "CrickSoftware – Clicker – Palette",
                MimeType = "application/vnd.crick.clicker.palette",
            },
            new ApacheMime()
            {
                Extension = ".clkt",
                Name = "CrickSoftware – Clicker – Template",
                MimeType = "application/vnd.crick.clicker.template",
            },
            new ApacheMime()
            {
                Extension = ".clkw",
                Name = "CrickSoftware – Clicker – Wordbank",
                MimeType = "application/vnd.crick.clicker.wordbank",
            },
            new ApacheMime()
            {
                Extension = ".wbs",
                Name = "Critical Tools – PERT Chart EXPERT",
                MimeType = "application/vnd.criticaltools.wbs+xml",
            },
            new ApacheMime()
            {
                Extension = ".cryptonote",
                Name = "CryptoNote",
                MimeType = "application/vnd.rig.cryptonote",
            },
            new ApacheMime()
            {
                Extension = ".cif",
                Name = "Crystallographic Interchange Format",
                MimeType = "chemical/x-cif",
            },
            new ApacheMime()
            {
                Extension = ".cmdf",
                Name = "CrystalMaker Data Format",
                MimeType = "chemical/x-cmdf",
            },
            new ApacheMime()
            {
                Extension = ".cu",
                Name = "CU-SeeMe",
                MimeType = "application/cu-seeme",
            },
            new ApacheMime()
            {
                Extension = ".cww",
                Name = "CU-Writer",
                MimeType = "application/prs.cww",
            },
            new ApacheMime()
            {
                Extension = ".curl",
                Name = "Curl – Applet",
                MimeType = "text/vnd.curl",
            },
            new ApacheMime()
            {
                Extension = ".dcurl",
                Name = "Curl – Detached Applet",
                MimeType = "text/vnd.curl.dcurl",
            },
            new ApacheMime()
            {
                Extension = ".mcurl",
                Name = "Curl – Manifest File",
                MimeType = "text/vnd.curl.mcurl",
            },
            new ApacheMime()
            {
                Extension = ".scurl",
                Name = "Curl – Source Code",
                MimeType = "text/vnd.curl.scurl",
            },
            new ApacheMime()
            {
                Extension = ".car",
                Name = "CURL Applet",
                MimeType = "application/vnd.curl.car",
            },
            new ApacheMime()
            {
                Extension = ".pcurl",
                Name = "CURL Applet",
                MimeType = "application/vnd.curl.pcurl",
            },
            new ApacheMime()
            {
                Extension = ".cmp",
                Name = "CustomMenu",
                MimeType = "application/vnd.yellowriver-custom-menu",
            },
            new ApacheMime()
            {
                Extension = ".dssc",
                Name = "Data Structure for the Security Suitability of Cryptographic Algorithms",
                MimeType = "application/dssc+der",
            },
            new ApacheMime()
            {
                Extension = ".xdssc",
                Name = "Data Structure for the Security Suitability of Cryptographic Algorithms",
                MimeType = "application/dssc+xml",
            },
            new ApacheMime()
            {
                Extension = ".deb",
                Name = "Debian Package",
                MimeType = "application/x-debian-package",
            },
            new ApacheMime()
            {
                Extension = ".uva",
                Name = "DECE Audio",
                MimeType = "audio/vnd.dece.audio",
            },
            new ApacheMime()
            {
                Extension = ".uvi",
                Name = "DECE Graphic",
                MimeType = "image/vnd.dece.graphic",
            },
            new ApacheMime()
            {
                Extension = ".uvh",
                Name = "DECE High Definition Video",
                MimeType = "video/vnd.dece.hd",
            },
            new ApacheMime()
            {
                Extension = ".uvm",
                Name = "DECE Mobile Video",
                MimeType = "video/vnd.dece.mobile",
            },
            new ApacheMime()
            {
                Extension = ".uvu",
                Name = "DECE MP4",
                MimeType = "video/vnd.uvvu.mp4",
            },
            new ApacheMime()
            {
                Extension = ".uvp",
                Name = "DECE PD Video",
                MimeType = "video/vnd.dece.pd",
            },
            new ApacheMime()
            {
                Extension = ".uvs",
                Name = "DECE SD Video",
                MimeType = "video/vnd.dece.sd",
            },
            new ApacheMime()
            {
                Extension = ".uvv",
                Name = "DECE Video",
                MimeType = "video/vnd.dece.video",
            },
            new ApacheMime()
            {
                Extension = ".dvi",
                Name = "Device Independent File Format (DVI)",
                MimeType = "application/x-dvi",
            },
            new ApacheMime()
            {
                Extension = ".seed",
                Name = "Digital Siesmograph Networks – SEED Datafiles",
                MimeType = "application/vnd.fdsn.seed",
            },
            new ApacheMime()
            {
                Extension = ".dtb",
                Name = "Digital Talking Book",
                MimeType = "application/x-dtbook+xml",
            },
            new ApacheMime()
            {
                Extension = ".res",
                Name = "Digital Talking Book – Resource File",
                MimeType = "application/x-dtbresource+xml",
            },
            new ApacheMime()
            {
                Extension = ".ait",
                Name = "Digital Video Broadcasting",
                MimeType = "application/vnd.dvb.ait",
            },
            new ApacheMime()
            {
                Extension = ".svc",
                Name = "Digital Video Broadcasting",
                MimeType = "application/vnd.dvb.service",
            },
            new ApacheMime()
            {
                Extension = ".eol",
                Name = "Digital Winds Music",
                MimeType = "audio/vnd.digital-winds",
            },
            new ApacheMime()
            {
                Extension = ".djvu",
                Name = "DjVu",
                MimeType = "image/vnd.djvu",
            },
            new ApacheMime()
            {
                Extension = ".dtd",
                Name = "Document Type Definition",
                MimeType = "application/xml-dtd",
            },
            new ApacheMime()
            {
                Extension = ".mlp",
                Name = "Dolby Meridian Lossless Packing",
                MimeType = "application/vnd.dolby.mlp",
            },
            new ApacheMime()
            {
                Extension = ".wad",
                Name = "Doom Video Game",
                MimeType = "application/x-doom",
            },
            new ApacheMime()
            {
                Extension = ".dpg",
                Name = "DPGraph",
                MimeType = "application/vnd.dpgraph",
            },
            new ApacheMime()
            {
                Extension = ".dra",
                Name = "DRA Audio",
                MimeType = "audio/vnd.dra",
            },
            new ApacheMime()
            {
                Extension = ".dfac",
                Name = "DreamFactory",
                MimeType = "application/vnd.dreamfactory",
            },
            new ApacheMime()
            {
                Extension = ".dts",
                Name = "DTS Audio",
                MimeType = "audio/vnd.dts",
            },
            new ApacheMime()
            {
                Extension = ".dtshd",
                Name = "DTS High Definition Audio",
                MimeType = "audio/vnd.dts.hd",
            },
            new ApacheMime()
            {
                Extension = ".dwg",
                Name = "DWG Drawing",
                MimeType = "image/vnd.dwg",
            },
            new ApacheMime()
            {
                Extension = ".geo",
                Name = "DynaGeo",
                MimeType = "application/vnd.dynageo",
            },
            new ApacheMime()
            {
                Extension = ".es",
                Name = "ECMAScript",
                MimeType = "application/ecmascript",
            },
            new ApacheMime()
            {
                Extension = ".mag",
                Name = "EcoWin Chart",
                MimeType = "application/vnd.ecowin.chart",
            },
            new ApacheMime()
            {
                Extension = ".mmr",
                Name = "EDMICS 2000",
                MimeType = "image/vnd.fujixerox.edmics-mmr",
            },
            new ApacheMime()
            {
                Extension = ".rlc",
                Name = "EDMICS 2000",
                MimeType = "image/vnd.fujixerox.edmics-rlc",
            },
            new ApacheMime()
            {
                Extension = ".exi",
                Name = "Efficient XML Interchange",
                MimeType = "application/exi",
            },
            new ApacheMime()
            {
                Extension = ".mgz",
                Name = "EFI Proteus",
                MimeType = "application/vnd.proteus.magazine",
            },
            new ApacheMime()
            {
                Extension = ".epub",
                Name = "Electronic Publication",
                MimeType = "application/epub+zip",
            },
            new ApacheMime()
            {
                Extension = ".eml",
                Name = "Email Message",
                MimeType = "message/rfc822",
            },
            new ApacheMime()
            {
                Extension = ".nml",
                Name = "Enliven Viewer",
                MimeType = "application/vnd.enliven",
            },
            new ApacheMime()
            {
                Extension = ".xpr",
                Name = "Express by Infoseek",
                MimeType = "application/vnd.is-xpr",
            },
            new ApacheMime()
            {
                Extension = ".xif",
                Name = "eXtended Image File Format (XIFF)",
                MimeType = "image/vnd.xiff",
            },
            new ApacheMime()
            {
                Extension = ".xfdl",
                Name = "Extensible Forms Description Language",
                MimeType = "application/vnd.xfdl",
            },
            new ApacheMime()
            {
                Extension = ".emma",
                Name = "Extensible MultiModal Annotation",
                MimeType = "application/emma+xml",
            },
            new ApacheMime()
            {
                Extension = ".ez2",
                Name = "EZPix Secure Photo Album",
                MimeType = "application/vnd.ezpix-album",
            },
            new ApacheMime()
            {
                Extension = ".ez3",
                Name = "EZPix Secure Photo Album",
                MimeType = "application/vnd.ezpix-package",
            },
            new ApacheMime()
            {
                Extension = ".fst",
                Name = "FAST Search & Transfer ASA",
                MimeType = "image/vnd.fst",
            },
            new ApacheMime()
            {
                Extension = ".fvt",
                Name = "FAST Search & Transfer ASA",
                MimeType = "video/vnd.fvt",
            },
            new ApacheMime()
            {
                Extension = ".fbs",
                Name = "FastBid Sheet",
                MimeType = "image/vnd.fastbidsheet",
            },
            new ApacheMime()
            {
                Extension = ".fe_launch",
                Name = "FCS Express Layout Link",
                MimeType = "application/vnd.denovo.fcselayout-link",
            },
            new ApacheMime()
            {
                Extension = ".f4v",
                Name = "Flash Video",
                MimeType = "video/x-f4v",
            },
            new ApacheMime()
            {
                Extension = ".flv",
                Name = "Flash Video",
                MimeType = "video/x-flv",
            },
            new ApacheMime()
            {
                Extension = ".fpx",
                Name = "FlashPix",
                MimeType = "image/vnd.fpx",
            },
            new ApacheMime()
            {
                Extension = ".npx",
                Name = "FlashPix",
                MimeType = "image/vnd.net-fpx",
            },
            new ApacheMime()
            {
                Extension = ".flx",
                Name = "FLEXSTOR",
                MimeType = "text/vnd.fmi.flexstor",
            },
            new ApacheMime()
            {
                Extension = ".fli",
                Name = "FLI/FLC Animation Format",
                MimeType = "video/x-fli",
            },
            new ApacheMime()
            {
                Extension = ".ftc",
                Name = "FluxTime Clip",
                MimeType = "application/vnd.fluxtime.clip",
            },
            new ApacheMime()
            {
                Extension = ".fdf",
                Name = "Forms Data Format",
                MimeType = "application/vnd.fdf",
            },
            new ApacheMime()
            {
                Extension = ".f",
                Name = "Fortran Source File",
                MimeType = "text/x-fortran",
            },
            new ApacheMime()
            {
                Extension = ".mif",
                Name = "FrameMaker Interchange Format",
                MimeType = "application/vnd.mif",
            },
            new ApacheMime()
            {
                Extension = ".fm",
                Name = "FrameMaker Normal Format",
                MimeType = "application/vnd.framemaker",
            },
            new ApacheMime()
            {
                Extension = ".fh",
                Name = "FreeHand MX",
                MimeType = "image/x-freehand",
            },
            new ApacheMime()
            {
                Extension = ".fsc",
                Name = "Friendly Software Corporation",
                MimeType = "application/vnd.fsc.weblaunch",
            },
            new ApacheMime()
            {
                Extension = ".fnc",
                Name = "Frogans Player",
                MimeType = "application/vnd.frogans.fnc",
            },
            new ApacheMime()
            {
                Extension = ".ltf",
                Name = "Frogans Player",
                MimeType = "application/vnd.frogans.ltf",
            },
            new ApacheMime()
            {
                Extension = ".ddd",
                Name = "Fujitsu – Xerox 2D CAD Data",
                MimeType = "application/vnd.fujixerox.ddd",
            },
            new ApacheMime()
            {
                Extension = ".xdw",
                Name = "Fujitsu – Xerox DocuWorks",
                MimeType = "application/vnd.fujixerox.docuworks",
            },
            new ApacheMime()
            {
                Extension = ".xbd",
                Name = "Fujitsu – Xerox DocuWorks Binder",
                MimeType = "application/vnd.fujixerox.docuworks.binder",
            },
            new ApacheMime()
            {
                Extension = ".oas",
                Name = "Fujitsu Oasys",
                MimeType = "application/vnd.fujitsu.oasys",
            },
            new ApacheMime()
            {
                Extension = ".oa2",
                Name = "Fujitsu Oasys",
                MimeType = "application/vnd.fujitsu.oasys2",
            },
            new ApacheMime()
            {
                Extension = ".oa3",
                Name = "Fujitsu Oasys",
                MimeType = "application/vnd.fujitsu.oasys3",
            },
            new ApacheMime()
            {
                Extension = ".fg5",
                Name = "Fujitsu Oasys",
                MimeType = "application/vnd.fujitsu.oasysgp",
            },
            new ApacheMime()
            {
                Extension = ".bh2",
                Name = "Fujitsu Oasys",
                MimeType = "application/vnd.fujitsu.oasysprs",
            },
            new ApacheMime()
            {
                Extension = ".spl",
                Name = "FutureSplash Animator",
                MimeType = "application/x-futuresplash",
            },
            new ApacheMime()
            {
                Extension = ".fzs",
                Name = "FuzzySheet",
                MimeType = "application/vnd.fuzzysheet",
            },
            new ApacheMime()
            {
                Extension = ".g3",
                Name = "G3 Fax Image",
                MimeType = "image/g3fax",
            },
            new ApacheMime()
            {
                Extension = ".gmx",
                Name = "GameMaker ActiveX",
                MimeType = "application/vnd.gmx",
            },
            new ApacheMime()
            {
                Extension = ".gtw",
                Name = "Gen-Trix Studio",
                MimeType = "model/vnd.gtw",
            },
            new ApacheMime()
            {
                Extension = ".txd",
                Name = "Genomatix Tuxedo Framework",
                MimeType = "application/vnd.genomatix.tuxedo",
            },
            new ApacheMime()
            {
                Extension = ".ggb",
                Name = "GeoGebra",
                MimeType = "application/vnd.geogebra.file",
            },
            new ApacheMime()
            {
                Extension = ".ggt",
                Name = "GeoGebra",
                MimeType = "application/vnd.geogebra.tool",
            },
            new ApacheMime()
            {
                Extension = ".gdl",
                Name = "Geometric Description Language (GDL)",
                MimeType = "model/vnd.gdl",
            },
            new ApacheMime()
            {
                Extension = ".gex",
                Name = "GeoMetry Explorer",
                MimeType = "application/vnd.geometry-explorer",
            },
            new ApacheMime()
            {
                Extension = ".gxt",
                Name = "GEONExT and JSXGraph",
                MimeType = "application/vnd.geonext",
            },
            new ApacheMime()
            {
                Extension = ".g2w",
                Name = "GeoplanW",
                MimeType = "application/vnd.geoplan",
            },
            new ApacheMime()
            {
                Extension = ".g3w",
                Name = "GeospacW",
                MimeType = "application/vnd.geospace",
            },
            new ApacheMime()
            {
                Extension = ".gsf",
                Name = "Ghostscript Font",
                MimeType = "application/x-font-ghostscript",
            },
            new ApacheMime()
            {
                Extension = ".bdf",
                Name = "Glyph Bitmap Distribution Format",
                MimeType = "application/x-font-bdf",
            },
            new ApacheMime()
            {
                Extension = ".gtar",
                Name = "GNU Tar Files",
                MimeType = "application/x-gtar",
            },
            new ApacheMime()
            {
                Extension = ".texinfo",
                Name = "GNU Texinfo Document",
                MimeType = "application/x-texinfo",
            },
            new ApacheMime()
            {
                Extension = ".gnumeric",
                Name = "Gnumeric",
                MimeType = "application/x-gnumeric",
            },
            new ApacheMime()
            {
                Extension = ".kml",
                Name = "Google Earth – KML",
                MimeType = "application/vnd.google-earth.kml+xml",
            },
            new ApacheMime()
            {
                Extension = ".kmz",
                Name = "Google Earth – Zipped KML",
                MimeType = "application/vnd.google-earth.kmz",
            },
            new ApacheMime()
            {
                Extension = ".gqf",
                Name = "GrafEq",
                MimeType = "application/vnd.grafeq",
            },
            new ApacheMime()
            {
                Extension = ".gif",
                Name = "Graphics Interchange Format",
                MimeType = "image/gif",
            },
            new ApacheMime()
            {
                Extension = ".gv",
                Name = "Graphviz",
                MimeType = "text/vnd.graphviz",
            },
            new ApacheMime()
            {
                Extension = ".gac",
                Name = "Groove – Account",
                MimeType = "application/vnd.groove-account",
            },
            new ApacheMime()
            {
                Extension = ".ghf",
                Name = "Groove – Help",
                MimeType = "application/vnd.groove-help",
            },
            new ApacheMime()
            {
                Extension = ".gim",
                Name = "Groove – Identity Message",
                MimeType = "application/vnd.groove-identity-message",
            },
            new ApacheMime()
            {
                Extension = ".grv",
                Name = "Groove – Injector",
                MimeType = "application/vnd.groove-injector",
            },
            new ApacheMime()
            {
                Extension = ".gtm",
                Name = "Groove – Tool Message",
                MimeType = "application/vnd.groove-tool-message",
            },
            new ApacheMime()
            {
                Extension = ".tpl",
                Name = "Groove – Tool Template",
                MimeType = "application/vnd.groove-tool-template",
            },
            new ApacheMime()
            {
                Extension = ".vcg",
                Name = "Groove – Vcard",
                MimeType = "application/vnd.groove-vcard",
            },
            new ApacheMime()
            {
                Extension = ".h261",
                Name = "H.261",
                MimeType = "video/h261",
            },
            new ApacheMime()
            {
                Extension = ".h263",
                Name = "H.263",
                MimeType = "video/h263",
            },
            new ApacheMime()
            {
                Extension = ".h264",
                Name = "H.264",
                MimeType = "video/h264",
            },
            new ApacheMime()
            {
                Extension = ".hpid",
                Name = "Hewlett Packard Instant Delivery",
                MimeType = "application/vnd.hp-hpid",
            },
            new ApacheMime()
            {
                Extension = ".hps",
                Name = "Hewlett-Packard’s WebPrintSmart",
                MimeType = "application/vnd.hp-hps",
            },
            new ApacheMime()
            {
                Extension = ".hdf",
                Name = "Hierarchical Data Format",
                MimeType = "application/x-hdf",
            },
            new ApacheMime()
            {
                Extension = ".rip",
                Name = "Hit’n’Mix",
                MimeType = "audio/vnd.rip",
            },
            new ApacheMime()
            {
                Extension = ".hbci",
                Name = "Homebanking Computer Interface (HBCI)",
                MimeType = "application/vnd.hbci",
            },
            new ApacheMime()
            {
                Extension = ".jlt",
                Name = "HP Indigo Digital Press – Job Layout Languate",
                MimeType = "application/vnd.hp-jlyt",
            },
            new ApacheMime()
            {
                Extension = ".pcl",
                Name = "HP Printer Command Language",
                MimeType = "application/vnd.hp-pcl",
            },
            new ApacheMime()
            {
                Extension = ".hpgl",
                Name = "HP-GL/2 and HP RTL",
                MimeType = "application/vnd.hp-hpgl",
            },
            new ApacheMime()
            {
                Extension = ".hvs",
                Name = "HV Script",
                MimeType = "application/vnd.yamaha.hv-script",
            },
            new ApacheMime()
            {
                Extension = ".hvd",
                Name = "HV Voice Dictionary",
                MimeType = "application/vnd.yamaha.hv-dic",
            },
            new ApacheMime()
            {
                Extension = ".hvp",
                Name = "HV Voice Parameter",
                MimeType = "application/vnd.yamaha.hv-voice",
            },
            new ApacheMime()
            {
                Extension = ".sfd-hdstx",
                Name = "Hydrostatix Master Suite",
                MimeType = "application/vnd.hydrostatix.sof-data",
            },
            new ApacheMime()
            {
                Extension = ".stk",
                Name = "Hyperstudio",
                MimeType = "application/hyperstudio",
            },
            new ApacheMime()
            {
                Extension = ".hal",
                Name = "Hypertext Application Language",
                MimeType = "application/vnd.hal+xml",
            },
            new ApacheMime()
            {
                Extension = ".html",
                Name = "HyperText Markup Language (HTML)",
                MimeType = "text/html",
            },
            new ApacheMime()
            {
                Extension = ".irm",
                Name = "IBM DB2 Rights Manager",
                MimeType = "application/vnd.ibm.rights-management",
            },
            new ApacheMime()
            {
                Extension = ".sc",
                Name = "IBM Electronic Media Management System – Secure Container",
                MimeType = "application/vnd.ibm.secure-container",
            },
            new ApacheMime()
            {
                Extension = ".ics",
                Name = "iCalendar",
                MimeType = "text/calendar",
            },
            new ApacheMime()
            {
                Extension = ".icc",
                Name = "ICC profile",
                MimeType = "application/vnd.iccprofile",
            },
            new ApacheMime()
            {
                Extension = ".ico",
                Name = "Icon Image",
                MimeType = "image/x-icon",
            },
            new ApacheMime()
            {
                Extension = ".igl",
                Name = "igLoader",
                MimeType = "application/vnd.igloader",
            },
            new ApacheMime()
            {
                Extension = ".ief",
                Name = "Image Exchange Format",
                MimeType = "image/ief",
            },
            new ApacheMime()
            {
                Extension = ".ivp",
                Name = "ImmerVision PURE Players",
                MimeType = "application/vnd.immervision-ivp",
            },
            new ApacheMime()
            {
                Extension = ".ivu",
                Name = "ImmerVision PURE Players",
                MimeType = "application/vnd.immervision-ivu",
            },
            new ApacheMime()
            {
                Extension = ".rif",
                Name = "IMS Networks",
                MimeType = "application/reginfo+xml",
            },
            new ApacheMime()
            {
                Extension = ".3dml",
                Name = "In3D – 3DML",
                MimeType = "text/vnd.in3d.3dml",
            },
            new ApacheMime()
            {
                Extension = ".spot",
                Name = "In3D – 3DML",
                MimeType = "text/vnd.in3d.spot",
            },
            new ApacheMime()
            {
                Extension = ".igs",
                Name = "Initial Graphics Exchange Specification (IGES)",
                MimeType = "model/iges",
            },
            new ApacheMime()
            {
                Extension = ".i2g",
                Name = "Interactive Geometry Software",
                MimeType = "application/vnd.intergeo",
            },
            new ApacheMime()
            {
                Extension = ".cdy",
                Name = "Interactive Geometry Software Cinderella",
                MimeType = "application/vnd.cinderella",
            },
            new ApacheMime()
            {
                Extension = ".xpw",
                Name = "Intercon FormNet",
                MimeType = "application/vnd.intercon.formnet",
            },
            new ApacheMime()
            {
                Extension = ".fcs",
                Name = "International Society for Advancement of Cytometry",
                MimeType = "application/vnd.isac.fcs",
            },
            new ApacheMime()
            {
                Extension = ".ipfix",
                Name = "Internet Protocol Flow Information Export",
                MimeType = "application/ipfix",
            },
            new ApacheMime()
            {
                Extension = ".cer",
                Name = "Internet Public Key Infrastructure – Certificate",
                MimeType = "application/pkix-cert",
            },
            new ApacheMime()
            {
                Extension = ".pki",
                Name = "Internet Public Key Infrastructure – Certificate Management Protocole",
                MimeType = "application/pkixcmp",
            },
            new ApacheMime()
            {
                Extension = ".crl",
                Name = "Internet Public Key Infrastructure – Certificate Revocation Lists",
                MimeType = "application/pkix-crl",
            },
            new ApacheMime()
            {
                Extension = ".pkipath",
                Name = "Internet Public Key Infrastructure – Certification Path",
                MimeType = "application/pkix-pkipath",
            },
            new ApacheMime()
            {
                Extension = ".igm",
                Name = "IOCOM Visimeet",
                MimeType = "application/vnd.insors.igm",
            },
            new ApacheMime()
            {
                Extension = ".rcprofile",
                Name = "IP Unplugged Roaming Client",
                MimeType = "application/vnd.ipunplugged.rcprofile",
            },
            new ApacheMime()
            {
                Extension = ".irp",
                Name = "iRepository / Lucidoc Editor",
                MimeType = "application/vnd.irepository.package+xml",
            },
            new ApacheMime()
            {
                Extension = ".jad",
                Name = "J2ME App Descriptor",
                MimeType = "text/vnd.sun.j2me.app-descriptor",
            },
            new ApacheMime()
            {
                Extension = ".jar",
                Name = "Java Archive",
                MimeType = "application/java-archive",
            },
            new ApacheMime()
            {
                Extension = ".class",
                Name = "Java Bytecode File",
                MimeType = "application/java-vm",
            },
            new ApacheMime()
            {
                Extension = ".jnlp",
                Name = "Java Network Launching Protocol",
                MimeType = "application/x-java-jnlp-file",
            },
            new ApacheMime()
            {
                Extension = ".ser",
                Name = "Java Serialized Object",
                MimeType = "application/java-serialized-object",
            },
            new ApacheMime()
            {
                Extension = ".java",
                Name = "Java Source File",
                MimeType = "text/x-java-source,java",
            },
            new ApacheMime()
            {
                Extension = ".js",
                Name = "JavaScript",
                MimeType = "application/javascript",
            },
            new ApacheMime()
            {
                Extension = ".json",
                Name = "JavaScript Object Notation (JSON)",
                MimeType = "application/json",
            },
            new ApacheMime()
            {
                Extension = ".joda",
                Name = "Joda Archive",
                MimeType = "application/vnd.joost.joda-archive",
            },
            new ApacheMime()
            {
                Extension = ".jpm",
                Name = "JPEG 2000 Compound Image File Format",
                MimeType = "video/jpm",
            },
            new ApacheMime()
            {
                Extension = ".jpeg, .jpg",
                Name = "JPEG Image",
                MimeType = "image/jpeg",
            },
            new ApacheMime()
            {
                Extension = ".jpgv",
                Name = "JPGVideo",
                MimeType = "video/jpeg",
            },
            new ApacheMime()
            {
                Extension = ".ktz",
                Name = "Kahootz",
                MimeType = "application/vnd.kahootz",
            },
            new ApacheMime()
            {
                Extension = ".mmd",
                Name = "Karaoke on Chipnuts Chipsets",
                MimeType = "application/vnd.chipnuts.karaoke-mmd",
            },
            new ApacheMime()
            {
                Extension = ".karbon",
                Name = "KDE KOffice Office Suite – Karbon",
                MimeType = "application/vnd.kde.karbon",
            },
            new ApacheMime()
            {
                Extension = ".chrt",
                Name = "KDE KOffice Office Suite – KChart",
                MimeType = "application/vnd.kde.kchart",
            },
            new ApacheMime()
            {
                Extension = ".kfo",
                Name = "KDE KOffice Office Suite – Kformula",
                MimeType = "application/vnd.kde.kformula",
            },
            new ApacheMime()
            {
                Extension = ".flw",
                Name = "KDE KOffice Office Suite – Kivio",
                MimeType = "application/vnd.kde.kivio",
            },
            new ApacheMime()
            {
                Extension = ".kon",
                Name = "KDE KOffice Office Suite – Kontour",
                MimeType = "application/vnd.kde.kontour",
            },
            new ApacheMime()
            {
                Extension = ".kpr",
                Name = "KDE KOffice Office Suite – Kpresenter",
                MimeType = "application/vnd.kde.kpresenter",
            },
            new ApacheMime()
            {
                Extension = ".ksp",
                Name = "KDE KOffice Office Suite – Kspread",
                MimeType = "application/vnd.kde.kspread",
            },
            new ApacheMime()
            {
                Extension = ".kwd",
                Name = "KDE KOffice Office Suite – Kword",
                MimeType = "application/vnd.kde.kword",
            },
            new ApacheMime()
            {
                Extension = ".htke",
                Name = "Kenamea App",
                MimeType = "application/vnd.kenameaapp",
            },
            new ApacheMime()
            {
                Extension = ".kia",
                Name = "Kidspiration",
                MimeType = "application/vnd.kidspiration",
            },
            new ApacheMime()
            {
                Extension = ".kne",
                Name = "Kinar Applications",
                MimeType = "application/vnd.kinar",
            },
            new ApacheMime()
            {
                Extension = ".sse",
                Name = "Kodak Storyshare",
                MimeType = "application/vnd.kodak-descriptor",
            },
            new ApacheMime()
            {
                Extension = ".lasxml",
                Name = "Laser App Enterprise",
                MimeType = "application/vnd.las.las+xml",
            },
            new ApacheMime()
            {
                Extension = ".latex",
                Name = "LaTeX",
                MimeType = "application/x-latex",
            },
            new ApacheMime()
            {
                Extension = ".lbd",
                Name = "Life Balance – Desktop Edition",
                MimeType = "application/vnd.llamagraphics.life-balance.desktop",
            },
            new ApacheMime()
            {
                Extension = ".lbe",
                Name = "Life Balance – Exchange Format",
                MimeType = "application/vnd.llamagraphics.life-balance.exchange+xml",
            },
            new ApacheMime()
            {
                Extension = ".jam",
                Name = "Lightspeed Audio Lab",
                MimeType = "application/vnd.jam",
            },
            new ApacheMime()
            {
                Extension = ".123",
                Name = "Lotus 1-2-3",
                MimeType = "application/vnd.lotus-1-2-3",
            },
            new ApacheMime()
            {
                Extension = ".apr",
                Name = "Lotus Approach",
                MimeType = "application/vnd.lotus-approach",
            },
            new ApacheMime()
            {
                Extension = ".pre",
                Name = "Lotus Freelance",
                MimeType = "application/vnd.lotus-freelance",
            },
            new ApacheMime()
            {
                Extension = ".nsf",
                Name = "Lotus Notes",
                MimeType = "application/vnd.lotus-notes",
            },
            new ApacheMime()
            {
                Extension = ".org",
                Name = "Lotus Organizer",
                MimeType = "application/vnd.lotus-organizer",
            },
            new ApacheMime()
            {
                Extension = ".scm",
                Name = "Lotus Screencam",
                MimeType = "application/vnd.lotus-screencam",
            },
            new ApacheMime()
            {
                Extension = ".lwp",
                Name = "Lotus Wordpro",
                MimeType = "application/vnd.lotus-wordpro",
            },
            new ApacheMime()
            {
                Extension = ".lvp",
                Name = "Lucent Voice",
                MimeType = "audio/vnd.lucent.voice",
            },
            new ApacheMime()
            {
                Extension = ".m3u",
                Name = "M3U (Multimedia Playlist)",
                MimeType = "audio/x-mpegurl",
            },
            new ApacheMime()
            {
                Extension = ".m4v",
                Name = "M4v",
                MimeType = "video/x-m4v",
            },
            new ApacheMime()
            {
                Extension = ".hqx",
                Name = "Macintosh BinHex 4.0",
                MimeType = "application/mac-binhex40",
            },
            new ApacheMime()
            {
                Extension = ".portpkg",
                Name = "MacPorts Port System",
                MimeType = "application/vnd.macports.portpkg",
            },
            new ApacheMime()
            {
                Extension = ".mgp",
                Name = "MapGuide DBXML",
                MimeType = "application/vnd.osgeo.mapguide.package",
            },
            new ApacheMime()
            {
                Extension = ".mrc",
                Name = "MARC Formats",
                MimeType = "application/marc",
            },
            new ApacheMime()
            {
                Extension = ".mrcx",
                Name = "MARC21 XML Schema",
                MimeType = "application/marcxml+xml",
            },
            new ApacheMime()
            {
                Extension = ".mxf",
                Name = "Material Exchange Format",
                MimeType = "application/mxf",
            },
            new ApacheMime()
            {
                Extension = ".nbp",
                Name = "Mathematica Notebook Player",
                MimeType = "application/vnd.wolfram.player",
            },
            new ApacheMime()
            {
                Extension = ".ma",
                Name = "Mathematica Notebooks",
                MimeType = "application/mathematica",
            },
            new ApacheMime()
            {
                Extension = ".mathml",
                Name = "Mathematical Markup Language",
                MimeType = "application/mathml+xml",
            },
            new ApacheMime()
            {
                Extension = ".mbox",
                Name = "Mbox database files",
                MimeType = "application/mbox",
            },
            new ApacheMime()
            {
                Extension = ".mc1",
                Name = "MedCalc",
                MimeType = "application/vnd.medcalcdata",
            },
            new ApacheMime()
            {
                Extension = ".mscml",
                Name = "Media Server Control Markup Language",
                MimeType = "application/mediaservercontrol+xml",
            },
            new ApacheMime()
            {
                Extension = ".cdkey",
                Name = "MediaRemote",
                MimeType = "application/vnd.mediastation.cdkey",
            },
            new ApacheMime()
            {
                Extension = ".mwf",
                Name = "Medical Waveform Encoding Format",
                MimeType = "application/vnd.mfer",
            },
            new ApacheMime()
            {
                Extension = ".mfm",
                Name = "Melody Format for Mobile Platform",
                MimeType = "application/vnd.mfmp",
            },
            new ApacheMime()
            {
                Extension = ".msh",
                Name = "Mesh Data Type",
                MimeType = "model/mesh",
            },
            new ApacheMime()
            {
                Extension = ".mads",
                Name = "Metadata Authority Description Schema",
                MimeType = "application/mads+xml",
            },
            new ApacheMime()
            {
                Extension = ".mets",
                Name = "Metadata Encoding and Transmission Standard",
                MimeType = "application/mets+xml",
            },
            new ApacheMime()
            {
                Extension = ".mods",
                Name = "Metadata Object Description Schema",
                MimeType = "application/mods+xml",
            },
            new ApacheMime()
            {
                Extension = ".meta4",
                Name = "Metalink",
                MimeType = "application/metalink4+xml",
            },
            new ApacheMime()
            {
                Extension = ".potm",
                Name = "Micosoft PowerPoint – Macro-Enabled Template File",
                MimeType = "application/vnd.ms-powerpoint.template.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".docm",
                Name = "Micosoft Word – Macro-Enabled Document",
                MimeType = "application/vnd.ms-word.document.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".dotm",
                Name = "Micosoft Word – Macro-Enabled Template",
                MimeType = "application/vnd.ms-word.template.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".mcd",
                Name = "Micro CADAM Helix D&D",
                MimeType = "application/vnd.mcd",
            },
            new ApacheMime()
            {
                Extension = ".flo",
                Name = "Micrografx",
                MimeType = "application/vnd.micrografx.flo",
            },
            new ApacheMime()
            {
                Extension = ".igx",
                Name = "Micrografx iGrafx Professional",
                MimeType = "application/vnd.micrografx.igx",
            },
            new ApacheMime()
            {
                Extension = ".es3",
                Name = "MICROSEC e-Szign¢",
                MimeType = "application/vnd.eszigno3+xml",
            },
            new ApacheMime()
            {
                Extension = ".mdb",
                Name = "Microsoft Access",
                MimeType = "application/x-msaccess",
            },
            new ApacheMime()
            {
                Extension = ".asf",
                Name = "Microsoft Advanced Systems Format (ASF)",
                MimeType = "video/x-ms-asf",
            },
            new ApacheMime()
            {
                Extension = ".exe",
                Name = "Microsoft Application",
                MimeType = "application/x-msdownload",
            },
            new ApacheMime()
            {
                Extension = ".cil",
                Name = "Microsoft Artgalry",
                MimeType = "application/vnd.ms-artgalry",
            },
            new ApacheMime()
            {
                Extension = ".cab",
                Name = "Microsoft Cabinet File",
                MimeType = "application/vnd.ms-cab-compressed",
            },
            new ApacheMime()
            {
                Extension = ".ims",
                Name = "Microsoft Class Server",
                MimeType = "application/vnd.ms-ims",
            },
            new ApacheMime()
            {
                Extension = ".application",
                Name = "Microsoft ClickOnce",
                MimeType = "application/x-ms-application",
            },
            new ApacheMime()
            {
                Extension = ".clp",
                Name = "Microsoft Clipboard Clip",
                MimeType = "application/x-msclip",
            },
            new ApacheMime()
            {
                Extension = ".mdi",
                Name = "Microsoft Document Imaging Format",
                MimeType = "image/vnd.ms-modi",
            },
            new ApacheMime()
            {
                Extension = ".eot",
                Name = "Microsoft Embedded OpenType",
                MimeType = "application/vnd.ms-fontobject",
            },
            new ApacheMime()
            {
                Extension = ".xls",
                Name = "Microsoft Excel",
                MimeType = "application/vnd.ms-excel",
            },
            new ApacheMime()
            {
                Extension = ".xlam",
                Name = "Microsoft Excel – Add-In File",
                MimeType = "application/vnd.ms-excel.addin.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".xlsb",
                Name = "Microsoft Excel – Binary Workbook",
                MimeType = "application/vnd.ms-excel.sheet.binary.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".xltm",
                Name = "Microsoft Excel – Macro-Enabled Template File",
                MimeType = "application/vnd.ms-excel.template.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".xlsm",
                Name = "Microsoft Excel – Macro-Enabled Workbook",
                MimeType = "application/vnd.ms-excel.sheet.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".chm",
                Name = "Microsoft Html Help File",
                MimeType = "application/vnd.ms-htmlhelp",
            },
            new ApacheMime()
            {
                Extension = ".crd",
                Name = "Microsoft Information Card",
                MimeType = "application/x-mscardfile",
            },
            new ApacheMime()
            {
                Extension = ".lrm",
                Name = "Microsoft Learning Resource Module",
                MimeType = "application/vnd.ms-lrm",
            },
            new ApacheMime()
            {
                Extension = ".mvb",
                Name = "Microsoft MediaView",
                MimeType = "application/x-msmediaview",
            },
            new ApacheMime()
            {
                Extension = ".mny",
                Name = "Microsoft Money",
                MimeType = "application/x-msmoney",
            },
            new ApacheMime()
            {
                Extension = ".pptx",
                Name = "Microsoft Office – OOXML – Presentation",
                MimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            },
            new ApacheMime()
            {
                Extension = ".sldx",
                Name = "Microsoft Office – OOXML – Presentation (Slide)",
                MimeType = "application/vnd.openxmlformats-officedocument.presentationml.slide",
            },
            new ApacheMime()
            {
                Extension = ".ppsx",
                Name = "Microsoft Office – OOXML – Presentation (Slideshow)",
                MimeType = "application/vnd.openxmlformats-officedocument.presentationml.slideshow",
            },
            new ApacheMime()
            {
                Extension = ".potx",
                Name = "Microsoft Office – OOXML – Presentation Template",
                MimeType = "application/vnd.openxmlformats-officedocument.presentationml.template",
            },
            new ApacheMime()
            {
                Extension = ".xlsx",
                Name = "Microsoft Office – OOXML – Spreadsheet",
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            },
            new ApacheMime()
            {
                Extension = ".xltx",
                Name = "Microsoft Office – OOXML – Spreadsheet Teplate",
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.template",
            },
            new ApacheMime()
            {
                Extension = ".docx",
                Name = "Microsoft Office – OOXML – Word Document",
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            },
            new ApacheMime()
            {
                Extension = ".dotx",
                Name = "Microsoft Office – OOXML – Word Document Template",
                MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template",
            },
            new ApacheMime()
            {
                Extension = ".obd",
                Name = "Microsoft Office Binder",
                MimeType = "application/x-msbinder",
            },
            new ApacheMime()
            {
                Extension = ".thmx",
                Name = "Microsoft Office System Release Theme",
                MimeType = "application/vnd.ms-officetheme",
            },
            new ApacheMime()
            {
                Extension = ".onetoc",
                Name = "Microsoft OneNote",
                MimeType = "application/onenote",
            },
            new ApacheMime()
            {
                Extension = ".pya",
                Name = "Microsoft PlayReady Ecosystem",
                MimeType = "audio/vnd.ms-playready.media.pya",
            },
            new ApacheMime()
            {
                Extension = ".pyv",
                Name = "Microsoft PlayReady Ecosystem Video",
                MimeType = "video/vnd.ms-playready.media.pyv",
            },
            new ApacheMime()
            {
                Extension = ".ppt",
                Name = "Microsoft PowerPoint",
                MimeType = "application/vnd.ms-powerpoint",
            },
            new ApacheMime()
            {
                Extension = ".ppam",
                Name = "Microsoft PowerPoint – Add-in file",
                MimeType = "application/vnd.ms-powerpoint.addin.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".sldm",
                Name = "Microsoft PowerPoint – Macro-Enabled Open XML Slide",
                MimeType = "application/vnd.ms-powerpoint.slide.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".pptm",
                Name = "Microsoft PowerPoint – Macro-Enabled Presentation File",
                MimeType = "application/vnd.ms-powerpoint.presentation.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".ppsm",
                Name = "Microsoft PowerPoint – Macro-Enabled Slide Show File",
                MimeType = "application/vnd.ms-powerpoint.slideshow.macroenabled.12",
            },
            new ApacheMime()
            {
                Extension = ".mpp",
                Name = "Microsoft Project",
                MimeType = "application/vnd.ms-project",
            },
            new ApacheMime()
            {
                Extension = ".pub",
                Name = "Microsoft Publisher",
                MimeType = "application/x-mspublisher",
            },
            new ApacheMime()
            {
                Extension = ".scd",
                Name = "Microsoft Schedule+",
                MimeType = "application/x-msschedule",
            },
            new ApacheMime()
            {
                Extension = ".xap",
                Name = "Microsoft Silverlight",
                MimeType = "application/x-silverlight-app",
            },
            new ApacheMime()
            {
                Extension = ".stl",
                Name = "Microsoft Trust UI Provider – Certificate Trust Link",
                MimeType = "application/vnd.ms-pki.stl",
            },
            new ApacheMime()
            {
                Extension = ".cat",
                Name = "Microsoft Trust UI Provider – Security Catalog",
                MimeType = "application/vnd.ms-pki.seccat",
            },
            new ApacheMime()
            {
                Extension = ".vsd",
                Name = "Microsoft Visio",
                MimeType = "application/vnd.visio",
            },
            new ApacheMime()
            {
                Extension = ".wm",
                Name = "Microsoft Windows Media",
                MimeType = "video/x-ms-wm",
            },
            new ApacheMime()
            {
                Extension = ".wma",
                Name = "Microsoft Windows Media Audio",
                MimeType = "audio/x-ms-wma",
            },
            new ApacheMime()
            {
                Extension = ".wax",
                Name = "Microsoft Windows Media Audio Redirector",
                MimeType = "audio/x-ms-wax",
            },
            new ApacheMime()
            {
                Extension = ".wmx",
                Name = "Microsoft Windows Media Audio/Video Playlist",
                MimeType = "video/x-ms-wmx",
            },
            new ApacheMime()
            {
                Extension = ".wmd",
                Name = "Microsoft Windows Media Player Download Package",
                MimeType = "application/x-ms-wmd",
            },
            new ApacheMime()
            {
                Extension = ".wpl",
                Name = "Microsoft Windows Media Player Playlist",
                MimeType = "application/vnd.ms-wpl",
            },
            new ApacheMime()
            {
                Extension = ".wmz",
                Name = "Microsoft Windows Media Player Skin Package",
                MimeType = "application/x-ms-wmz",
            },
            new ApacheMime()
            {
                Extension = ".wmv",
                Name = "Microsoft Windows Media Video",
                MimeType = "video/x-ms-wmv",
            },
            new ApacheMime()
            {
                Extension = ".wvx",
                Name = "Microsoft Windows Media Video Playlist",
                MimeType = "video/x-ms-wvx",
            },
            new ApacheMime()
            {
                Extension = ".wmf",
                Name = "Microsoft Windows Metafile",
                MimeType = "application/x-msmetafile",
            },
            new ApacheMime()
            {
                Extension = ".trm",
                Name = "Microsoft Windows Terminal Services",
                MimeType = "application/x-msterminal",
            },
            new ApacheMime()
            {
                Extension = ".doc",
                Name = "Microsoft Word",
                MimeType = "application/msword",
            },
            new ApacheMime()
            {
                Extension = ".wri",
                Name = "Microsoft Wordpad",
                MimeType = "application/x-mswrite",
            },
            new ApacheMime()
            {
                Extension = ".wps",
                Name = "Microsoft Works",
                MimeType = "application/vnd.ms-works",
            },
            new ApacheMime()
            {
                Extension = ".xbap",
                Name = "Microsoft XAML Browser Application",
                MimeType = "application/x-ms-xbap",
            },
            new ApacheMime()
            {
                Extension = ".xps",
                Name = "Microsoft XML Paper Specification",
                MimeType = "application/vnd.ms-xpsdocument",
            },
            new ApacheMime()
            {
                Extension = ".mid",
                Name = "MIDI – Musical Instrument Digital Interface",
                MimeType = "audio/midi",
            },
            new ApacheMime()
            {
                Extension = ".mpy",
                Name = "MiniPay",
                MimeType = "application/vnd.ibm.minipay",
            },
            new ApacheMime()
            {
                Extension = ".afp",
                Name = "MO:DCA-P",
                MimeType = "application/vnd.ibm.modcap",
            },
            new ApacheMime()
            {
                Extension = ".rms",
                Name = "Mobile Information Device Profile",
                MimeType = "application/vnd.jcp.javame.midlet-rms",
            },
            new ApacheMime()
            {
                Extension = ".tmo",
                Name = "MobileTV",
                MimeType = "application/vnd.tmobile-livetv",
            },
            new ApacheMime()
            {
                Extension = ".prc",
                Name = "Mobipocket",
                MimeType = "application/x-mobipocket-ebook",
            },
            new ApacheMime()
            {
                Extension = ".mbk",
                Name = "Mobius Management Systems – Basket file",
                MimeType = "application/vnd.mobius.mbk",
            },
            new ApacheMime()
            {
                Extension = ".dis",
                Name = "Mobius Management Systems – Distribution Database",
                MimeType = "application/vnd.mobius.dis",
            },
            new ApacheMime()
            {
                Extension = ".plc",
                Name = "Mobius Management Systems – Policy Definition Language File",
                MimeType = "application/vnd.mobius.plc",
            },
            new ApacheMime()
            {
                Extension = ".mqy",
                Name = "Mobius Management Systems – Query File",
                MimeType = "application/vnd.mobius.mqy",
            },
            new ApacheMime()
            {
                Extension = ".msl",
                Name = "Mobius Management Systems – Script Language",
                MimeType = "application/vnd.mobius.msl",
            },
            new ApacheMime()
            {
                Extension = ".txf",
                Name = "Mobius Management Systems – Topic Index File",
                MimeType = "application/vnd.mobius.txf",
            },
            new ApacheMime()
            {
                Extension = ".daf",
                Name = "Mobius Management Systems – UniversalArchive",
                MimeType = "application/vnd.mobius.daf",
            },
            new ApacheMime()
            {
                Extension = ".fly",
                Name = "mod_fly / fly.cgi",
                MimeType = "text/vnd.fly",
            },
            new ApacheMime()
            {
                Extension = ".mpc",
                Name = "Mophun Certificate",
                MimeType = "application/vnd.mophun.certificate",
            },
            new ApacheMime()
            {
                Extension = ".mpn",
                Name = "Mophun VM",
                MimeType = "application/vnd.mophun.application",
            },
            new ApacheMime()
            {
                Extension = ".mj2",
                Name = "Motion JPEG 2000",
                MimeType = "video/mj2",
            },
            new ApacheMime()
            {
                Extension = ".mpga",
                Name = "MPEG Audio",
                MimeType = "audio/mpeg",
            },
            new ApacheMime()
            {
                Extension = ".mxu",
                Name = "MPEG Url",
                MimeType = "video/vnd.mpegurl",
            },
            new ApacheMime()
            {
                Extension = ".mpeg",
                Name = "MPEG Video",
                MimeType = "video/mpeg",
            },
            new ApacheMime()
            {
                Extension = ".m21",
                Name = "MPEG-21",
                MimeType = "application/mp21",
            },
            new ApacheMime()
            {
                Extension = ".mp4a",
                Name = "MPEG-4 Audio",
                MimeType = "audio/mp4",
            },
            new ApacheMime()
            {
                Extension = ".mp4",
                Name = "MPEG-4 Video",
                MimeType = "video/mp4",
            },
            new ApacheMime()
            {
                Extension = ".mp4",
                Name = "MPEG4",
                MimeType = "application/mp4",
            },
            new ApacheMime()
            {
                Extension = ".m3u8",
                Name = "Multimedia Playlist Unicode",
                MimeType = "application/vnd.apple.mpegurl",
            },
            new ApacheMime()
            {
                Extension = ".mus",
                Name = "MUsical Score Interpreted Code Invented for the ASCII designation of Notation",
                MimeType = "application/vnd.musician",
            },
            new ApacheMime()
            {
                Extension = ".msty",
                Name = "Muvee Automatic Video Editing",
                MimeType = "application/vnd.muvee.style",
            },
            new ApacheMime()
            {
                Extension = ".mxml",
                Name = "MXML",
                MimeType = "application/xv+xml",
            },
            new ApacheMime()
            {
                Extension = ".ngdat",
                Name = "N-Gage Game Data",
                MimeType = "application/vnd.nokia.n-gage.data",
            },
            new ApacheMime()
            {
                Extension = ".n-gage",
                Name = "N-Gage Game Installer",
                MimeType = "application/vnd.nokia.n-gage.symbian.install",
            },
            new ApacheMime()
            {
                Extension = ".ncx",
                Name = "Navigation Control file for XML (for ePub)",
                MimeType = "application/x-dtbncx+xml",
            },
            new ApacheMime()
            {
                Extension = ".nc",
                Name = "Network Common Data Form (NetCDF)",
                MimeType = "application/x-netcdf",
            },
            new ApacheMime()
            {
                Extension = ".nlu",
                Name = "neuroLanguage",
                MimeType = "application/vnd.neurolanguage.nlu",
            },
            new ApacheMime()
            {
                Extension = ".dna",
                Name = "New Moon Liftoff/DNA",
                MimeType = "application/vnd.dna",
            },
            new ApacheMime()
            {
                Extension = ".nnd",
                Name = "NobleNet Directory",
                MimeType = "application/vnd.noblenet-directory",
            },
            new ApacheMime()
            {
                Extension = ".nns",
                Name = "NobleNet Sealer",
                MimeType = "application/vnd.noblenet-sealer",
            },
            new ApacheMime()
            {
                Extension = ".nnw",
                Name = "NobleNet Web",
                MimeType = "application/vnd.noblenet-web",
            },
            new ApacheMime()
            {
                Extension = ".rpst",
                Name = "Nokia Radio Application – Preset",
                MimeType = "application/vnd.nokia.radio-preset",
            },
            new ApacheMime()
            {
                Extension = ".rpss",
                Name = "Nokia Radio Application – Preset",
                MimeType = "application/vnd.nokia.radio-presets",
            },
            new ApacheMime()
            {
                Extension = ".n3",
                Name = "Notation3",
                MimeType = "text/n3",
            },
            new ApacheMime()
            {
                Extension = ".edm",
                Name = "Novadigm’s RADIA and EDM products",
                MimeType = "application/vnd.novadigm.edm",
            },
            new ApacheMime()
            {
                Extension = ".edx",
                Name = "Novadigm’s RADIA and EDM products",
                MimeType = "application/vnd.novadigm.edx",
            },
            new ApacheMime()
            {
                Extension = ".ext",
                Name = "Novadigm’s RADIA and EDM products",
                MimeType = "application/vnd.novadigm.ext",
            },
            new ApacheMime()
            {
                Extension = ".gph",
                Name = "NpGraphIt",
                MimeType = "application/vnd.flographit",
            },
            new ApacheMime()
            {
                Extension = ".ecelp4800",
                Name = "Nuera ECELP 4800",
                MimeType = "audio/vnd.nuera.ecelp4800",
            },
            new ApacheMime()
            {
                Extension = ".ecelp7470",
                Name = "Nuera ECELP 7470",
                MimeType = "audio/vnd.nuera.ecelp7470",
            },
            new ApacheMime()
            {
                Extension = ".ecelp9600",
                Name = "Nuera ECELP 9600",
                MimeType = "audio/vnd.nuera.ecelp9600",
            },
            new ApacheMime()
            {
                Extension = ".oda",
                Name = "Office Document Architecture",
                MimeType = "application/oda",
            },
            new ApacheMime()
            {
                Extension = ".ogx",
                Name = "Ogg",
                MimeType = "application/ogg",
            },
            new ApacheMime()
            {
                Extension = ".oga",
                Name = "Ogg Audio",
                MimeType = "audio/ogg",
            },
            new ApacheMime()
            {
                Extension = ".ogv",
                Name = "Ogg Video",
                MimeType = "video/ogg",
            },
            new ApacheMime()
            {
                Extension = ".dd2",
                Name = "OMA Download Agents",
                MimeType = "application/vnd.oma.dd2+xml",
            },
            new ApacheMime()
            {
                Extension = ".oth",
                Name = "Open Document Text Web",
                MimeType = "application/vnd.oasis.opendocument.text-web",
            },
            new ApacheMime()
            {
                Extension = ".opf",
                Name = "Open eBook Publication Structure",
                MimeType = "application/oebps-package+xml",
            },
            new ApacheMime()
            {
                Extension = ".qbo",
                Name = "Open Financial Exchange",
                MimeType = "application/vnd.intu.qbo",
            },
            new ApacheMime()
            {
                Extension = ".oxt",
                Name = "Open Office Extension",
                MimeType = "application/vnd.openofficeorg.extension",
            },
            new ApacheMime()
            {
                Extension = ".osf",
                Name = "Open Score Format",
                MimeType = "application/vnd.yamaha.openscoreformat",
            },
            new ApacheMime()
            {
                Extension = ".weba",
                Name = "Open Web Media Project – Audio",
                MimeType = "audio/webm",
            },
            new ApacheMime()
            {
                Extension = ".webm",
                Name = "Open Web Media Project – Video",
                MimeType = "video/webm",
            },
            new ApacheMime()
            {
                Extension = ".odc",
                Name = "OpenDocument Chart",
                MimeType = "application/vnd.oasis.opendocument.chart",
            },
            new ApacheMime()
            {
                Extension = ".otc",
                Name = "OpenDocument Chart Template",
                MimeType = "application/vnd.oasis.opendocument.chart-template",
            },
            new ApacheMime()
            {
                Extension = ".odb",
                Name = "OpenDocument Database",
                MimeType = "application/vnd.oasis.opendocument.database",
            },
            new ApacheMime()
            {
                Extension = ".odf",
                Name = "OpenDocument Formula",
                MimeType = "application/vnd.oasis.opendocument.formula",
            },
            new ApacheMime()
            {
                Extension = ".odft",
                Name = "OpenDocument Formula Template",
                MimeType = "application/vnd.oasis.opendocument.formula-template",
            },
            new ApacheMime()
            {
                Extension = ".odg",
                Name = "OpenDocument Graphics",
                MimeType = "application/vnd.oasis.opendocument.graphics",
            },
            new ApacheMime()
            {
                Extension = ".otg",
                Name = "OpenDocument Graphics Template",
                MimeType = "application/vnd.oasis.opendocument.graphics-template",
            },
            new ApacheMime()
            {
                Extension = ".odi",
                Name = "OpenDocument Image",
                MimeType = "application/vnd.oasis.opendocument.image",
            },
            new ApacheMime()
            {
                Extension = ".oti",
                Name = "OpenDocument Image Template",
                MimeType = "application/vnd.oasis.opendocument.image-template",
            },
            new ApacheMime()
            {
                Extension = ".odp",
                Name = "OpenDocument Presentation",
                MimeType = "application/vnd.oasis.opendocument.presentation",
            },
            new ApacheMime()
            {
                Extension = ".otp",
                Name = "OpenDocument Presentation Template",
                MimeType = "application/vnd.oasis.opendocument.presentation-template",
            },
            new ApacheMime()
            {
                Extension = ".ods",
                Name = "OpenDocument Spreadsheet",
                MimeType = "application/vnd.oasis.opendocument.spreadsheet",
            },
            new ApacheMime()
            {
                Extension = ".ots",
                Name = "OpenDocument Spreadsheet Template",
                MimeType = "application/vnd.oasis.opendocument.spreadsheet-template",
            },
            new ApacheMime()
            {
                Extension = ".odt",
                Name = "OpenDocument Text",
                MimeType = "application/vnd.oasis.opendocument.text",
            },
            new ApacheMime()
            {
                Extension = ".odm",
                Name = "OpenDocument Text Master",
                MimeType = "application/vnd.oasis.opendocument.text-master",
            },
            new ApacheMime()
            {
                Extension = ".ott",
                Name = "OpenDocument Text Template",
                MimeType = "application/vnd.oasis.opendocument.text-template",
            },
            new ApacheMime()
            {
                Extension = ".ktx",
                Name = "OpenGL Textures (KTX)",
                MimeType = "image/ktx",
            },
            new ApacheMime()
            {
                Extension = ".sxc",
                Name = "OpenOffice – Calc (Spreadsheet)",
                MimeType = "application/vnd.sun.xml.calc",
            },
            new ApacheMime()
            {
                Extension = ".stc",
                Name = "OpenOffice – Calc Template (Spreadsheet)",
                MimeType = "application/vnd.sun.xml.calc.template",
            },
            new ApacheMime()
            {
                Extension = ".sxd",
                Name = "OpenOffice – Draw (Graphics)",
                MimeType = "application/vnd.sun.xml.draw",
            },
            new ApacheMime()
            {
                Extension = ".std",
                Name = "OpenOffice – Draw Template (Graphics)",
                MimeType = "application/vnd.sun.xml.draw.template",
            },
            new ApacheMime()
            {
                Extension = ".sxi",
                Name = "OpenOffice – Impress (Presentation)",
                MimeType = "application/vnd.sun.xml.impress",
            },
            new ApacheMime()
            {
                Extension = ".sti",
                Name = "OpenOffice – Impress Template (Presentation)",
                MimeType = "application/vnd.sun.xml.impress.template",
            },
            new ApacheMime()
            {
                Extension = ".sxm",
                Name = "OpenOffice – Math (Formula)",
                MimeType = "application/vnd.sun.xml.math",
            },
            new ApacheMime()
            {
                Extension = ".sxw",
                Name = "OpenOffice – Writer (Text – HTML)",
                MimeType = "application/vnd.sun.xml.writer",
            },
            new ApacheMime()
            {
                Extension = ".sxg",
                Name = "OpenOffice – Writer (Text – HTML)",
                MimeType = "application/vnd.sun.xml.writer.global",
            },
            new ApacheMime()
            {
                Extension = ".stw",
                Name = "OpenOffice – Writer Template (Text – HTML)",
                MimeType = "application/vnd.sun.xml.writer.template",
            },
            new ApacheMime()
            {
                Extension = ".otf",
                Name = "OpenType Font File",
                MimeType = "application/x-font-otf",
            },
            new ApacheMime()
            {
                Extension = ".osfpvg",
                Name = "OSFPVG",
                MimeType = "application/vnd.yamaha.openscoreformat.osfpvg+xml",
            },
            new ApacheMime()
            {
                Extension = ".dp",
                Name = "OSGi Deployment Package",
                MimeType = "application/vnd.osgi.dp",
            },
            new ApacheMime()
            {
                Extension = ".pdb",
                Name = "PalmOS Data",
                MimeType = "application/vnd.palm",
            },
            new ApacheMime()
            {
                Extension = ".p",
                Name = "Pascal Source File",
                MimeType = "text/x-pascal",
            },
            new ApacheMime()
            {
                Extension = ".paw",
                Name = "PawaaFILE",
                MimeType = "application/vnd.pawaafile",
            },
            new ApacheMime()
            {
                Extension = ".pclxl",
                Name = "PCL 6 Enhanced (Formely PCL XL)",
                MimeType = "application/vnd.hp-pclxl",
            },
            new ApacheMime()
            {
                Extension = ".efif",
                Name = "Pcsel eFIF File",
                MimeType = "application/vnd.picsel",
            },
            new ApacheMime()
            {
                Extension = ".pcx",
                Name = "PCX Image",
                MimeType = "image/x-pcx",
            },
            new ApacheMime()
            {
                Extension = ".psd",
                Name = "Photoshop Document",
                MimeType = "image/vnd.adobe.photoshop",
            },
            new ApacheMime()
            {
                Extension = ".prf",
                Name = "PICSRules",
                MimeType = "application/pics-rules",
            },
            new ApacheMime()
            {
                Extension = ".pic",
                Name = "PICT Image",
                MimeType = "image/x-pict",
            },
            new ApacheMime()
            {
                Extension = ".chat",
                Name = "pIRCh",
                MimeType = "application/x-chat",
            },
            new ApacheMime()
            {
                Extension = ".p10",
                Name = "PKCS #10 – Certification Request Standard",
                MimeType = "application/pkcs10",
            },
            new ApacheMime()
            {
                Extension = ".p12",
                Name = "PKCS #12 – Personal Information Exchange Syntax Standard",
                MimeType = "application/x-pkcs12",
            },
            new ApacheMime()
            {
                Extension = ".p7m",
                Name = "PKCS #7 – Cryptographic Message Syntax Standard",
                MimeType = "application/pkcs7-mime",
            },
            new ApacheMime()
            {
                Extension = ".p7s",
                Name = "PKCS #7 – Cryptographic Message Syntax Standard",
                MimeType = "application/pkcs7-signature",
            },
            new ApacheMime()
            {
                Extension = ".p7r",
                Name = "PKCS #7 – Cryptographic Message Syntax Standard (Certificate Request Response)",
                MimeType = "application/x-pkcs7-certreqresp",
            },
            new ApacheMime()
            {
                Extension = ".p7b",
                Name = "PKCS #7 – Cryptographic Message Syntax Standard (Certificates)",
                MimeType = "application/x-pkcs7-certificates",
            },
            new ApacheMime()
            {
                Extension = ".p8",
                Name = "PKCS #8 – Private-Key Information Syntax Standard",
                MimeType = "application/pkcs8",
            },
            new ApacheMime()
            {
                Extension = ".plf",
                Name = "PocketLearn Viewers",
                MimeType = "application/vnd.pocketlearn",
            },
            new ApacheMime()
            {
                Extension = ".pnm",
                Name = "Portable Anymap Image",
                MimeType = "image/x-portable-anymap",
            },
            new ApacheMime()
            {
                Extension = ".pbm",
                Name = "Portable Bitmap Format",
                MimeType = "image/x-portable-bitmap",
            },
            new ApacheMime()
            {
                Extension = ".pcf",
                Name = "Portable Compiled Format",
                MimeType = "application/x-font-pcf",
            },
            new ApacheMime()
            {
                Extension = ".pfr",
                Name = "Portable Font Resource",
                MimeType = "application/font-tdpfr",
            },
            new ApacheMime()
            {
                Extension = ".pgn",
                Name = "Portable Game Notation (Chess Games)",
                MimeType = "application/x-chess-pgn",
            },
            new ApacheMime()
            {
                Extension = ".pgm",
                Name = "Portable Graymap Format",
                MimeType = "image/x-portable-graymap",
            },
            new ApacheMime()
            {
                Extension = ".png",
                Name = "Portable Network Graphics (PNG)",
                MimeType = "image/png",
            },
            new ApacheMime()
            {
                Extension = ".ppm",
                Name = "Portable Pixmap Format",
                MimeType = "image/x-portable-pixmap",
            },
            new ApacheMime()
            {
                Extension = ".pskcxml",
                Name = "Portable Symmetric Key Container",
                MimeType = "application/pskc+xml",
            },
            new ApacheMime()
            {
                Extension = ".pml",
                Name = "PosML",
                MimeType = "application/vnd.ctc-posml",
            },
            new ApacheMime()
            {
                Extension = ".ai",
                Name = "PostScript",
                MimeType = "application/postscript",
            },
            new ApacheMime()
            {
                Extension = ".pfa",
                Name = "PostScript Fonts",
                MimeType = "application/x-font-type1",
            },
            new ApacheMime()
            {
                Extension = ".pbd",
                Name = "PowerBuilder",
                MimeType = "application/vnd.powerbuilder6",
            },
            new ApacheMime()
            {
                Extension = "",
                Name = "Pretty Good Privacy",
                MimeType = "application/pgp-encrypted",
            },
            new ApacheMime()
            {
                Extension = ".pgp",
                Name = "Pretty Good Privacy – Signature",
                MimeType = "application/pgp-signature",
            },
            new ApacheMime()
            {
                Extension = ".box",
                Name = "Preview Systems ZipLock/VBox",
                MimeType = "application/vnd.previewsystems.box",
            },
            new ApacheMime()
            {
                Extension = ".ptid",
                Name = "Princeton Video Image",
                MimeType = "application/vnd.pvi.ptid1",
            },
            new ApacheMime()
            {
                Extension = ".pls",
                Name = "Pronunciation Lexicon Specification",
                MimeType = "application/pls+xml",
            },
            new ApacheMime()
            {
                Extension = ".str",
                Name = "Proprietary P&G Standard Reporting System",
                MimeType = "application/vnd.pg.format",
            },
            new ApacheMime()
            {
                Extension = ".ei6",
                Name = "Proprietary P&G Standard Reporting System",
                MimeType = "application/vnd.pg.osasli",
            },
            new ApacheMime()
            {
                Extension = ".dsc",
                Name = "PRS Lines Tag",
                MimeType = "text/prs.lines.tag",
            },
            new ApacheMime()
            {
                Extension = ".psf",
                Name = "PSF Fonts",
                MimeType = "application/x-font-linux-psf",
            },
            new ApacheMime()
            {
                Extension = ".qps",
                Name = "PubliShare Objects",
                MimeType = "application/vnd.publishare-delta-tree",
            },
            new ApacheMime()
            {
                Extension = ".wg",
                Name = "Qualcomm’s Plaza Mobile Internet",
                MimeType = "application/vnd.pmi.widget",
            },
            new ApacheMime()
            {
                Extension = ".qxd",
                Name = "QuarkXpress",
                MimeType = "application/vnd.quark.quarkxpress",
            },
            new ApacheMime()
            {
                Extension = ".esf",
                Name = "QUASS Stream Player",
                MimeType = "application/vnd.epson.esf",
            },
            new ApacheMime()
            {
                Extension = ".msf",
                Name = "QUASS Stream Player",
                MimeType = "application/vnd.epson.msf",
            },
            new ApacheMime()
            {
                Extension = ".ssf",
                Name = "QUASS Stream Player",
                MimeType = "application/vnd.epson.ssf",
            },
            new ApacheMime()
            {
                Extension = ".qam",
                Name = "QuickAnime Player",
                MimeType = "application/vnd.epson.quickanime",
            },
            new ApacheMime()
            {
                Extension = ".qfx",
                Name = "Quicken",
                MimeType = "application/vnd.intu.qfx",
            },
            new ApacheMime()
            {
                Extension = ".qt",
                Name = "Quicktime Video",
                MimeType = "video/quicktime",
            },
            new ApacheMime()
            {
                Extension = ".rar",
                Name = "RAR Archive",
                MimeType = "application/x-rar-compressed",
            },
            new ApacheMime()
            {
                Extension = ".ram",
                Name = "Real Audio Sound",
                MimeType = "audio/x-pn-realaudio",
            },
            new ApacheMime()
            {
                Extension = ".rmp",
                Name = "Real Audio Sound",
                MimeType = "audio/x-pn-realaudio-plugin",
            },
            new ApacheMime()
            {
                Extension = ".rsd",
                Name = "Really Simple Discovery",
                MimeType = "application/rsd+xml",
            },
            new ApacheMime()
            {
                Extension = ".rm",
                Name = "RealMedia",
                MimeType = "application/vnd.rn-realmedia",
            },
            new ApacheMime()
            {
                Extension = ".bed",
                Name = "RealVNC",
                MimeType = "application/vnd.realvnc.bed",
            },
            new ApacheMime()
            {
                Extension = ".mxl",
                Name = "Recordare Applications",
                MimeType = "application/vnd.recordare.musicxml",
            },
            new ApacheMime()
            {
                Extension = ".musicxml",
                Name = "Recordare Applications",
                MimeType = "application/vnd.recordare.musicxml+xml",
            },
            new ApacheMime()
            {
                Extension = ".rnc",
                Name = "Relax NG Compact Syntax",
                MimeType = "application/relax-ng-compact-syntax",
            },
            new ApacheMime()
            {
                Extension = ".rdz",
                Name = "RemoteDocs R-Viewer",
                MimeType = "application/vnd.data-vision.rdz",
            },
            new ApacheMime()
            {
                Extension = ".rdf",
                Name = "Resource Description Framework",
                MimeType = "application/rdf+xml",
            },
            new ApacheMime()
            {
                Extension = ".rp9",
                Name = "RetroPlatform Player",
                MimeType = "application/vnd.cloanto.rp9",
            },
            new ApacheMime()
            {
                Extension = ".jisp",
                Name = "RhymBox",
                MimeType = "application/vnd.jisp",
            },
            new ApacheMime()
            {
                Extension = ".rtf",
                Name = "Rich Text Format",
                MimeType = "application/rtf",
            },
            new ApacheMime()
            {
                Extension = ".rtx",
                Name = "Rich Text Format (RTF)",
                MimeType = "text/richtext",
            },
            new ApacheMime()
            {
                Extension = ".link66",
                Name = "ROUTE 66 Location Based Services",
                MimeType = "application/vnd.route66.link66+xml",
            },
            new ApacheMime()
            {
                Extension = ".rss, .xml",
                Name = "RSS – Really Simple Syndication",
                MimeType = "application/rss+xml",
            },
            new ApacheMime()
            {
                Extension = ".shf",
                Name = "S Hexdump Format",
                MimeType = "application/shf+xml",
            },
            new ApacheMime()
            {
                Extension = ".st",
                Name = "SailingTracker",
                MimeType = "application/vnd.sailingtracker.track",
            },
            new ApacheMime()
            {
                Extension = ".svg",
                Name = "Scalable Vector Graphics (SVG)",
                MimeType = "image/svg+xml",
            },
            new ApacheMime()
            {
                Extension = ".sus",
                Name = "ScheduleUs",
                MimeType = "application/vnd.sus-calendar",
            },
            new ApacheMime()
            {
                Extension = ".sru",
                Name = "Search/Retrieve via URL Response Format",
                MimeType = "application/sru+xml",
            },
            new ApacheMime()
            {
                Extension = ".setpay",
                Name = "Secure Electronic Transaction – Payment",
                MimeType = "application/set-payment-initiation",
            },
            new ApacheMime()
            {
                Extension = ".setreg",
                Name = "Secure Electronic Transaction – Registration",
                MimeType = "application/set-registration-initiation",
            },
            new ApacheMime()
            {
                Extension = ".sema",
                Name = "Secured eMail",
                MimeType = "application/vnd.sema",
            },
            new ApacheMime()
            {
                Extension = ".semd",
                Name = "Secured eMail",
                MimeType = "application/vnd.semd",
            },
            new ApacheMime()
            {
                Extension = ".semf",
                Name = "Secured eMail",
                MimeType = "application/vnd.semf",
            },
            new ApacheMime()
            {
                Extension = ".see",
                Name = "SeeMail",
                MimeType = "application/vnd.seemail",
            },
            new ApacheMime()
            {
                Extension = ".snf",
                Name = "Server Normal Format",
                MimeType = "application/x-font-snf",
            },
            new ApacheMime()
            {
                Extension = ".spq",
                Name = "Server-Based Certificate Validation Protocol – Validation Policies – Request",
                MimeType = "application/scvp-vp-request",
            },
            new ApacheMime()
            {
                Extension = ".spp",
                Name = "Server-Based Certificate Validation Protocol – Validation Policies – Response",
                MimeType = "application/scvp-vp-response",
            },
            new ApacheMime()
            {
                Extension = ".scq",
                Name = "Server-Based Certificate Validation Protocol – Validation Request",
                MimeType = "application/scvp-cv-request",
            },
            new ApacheMime()
            {
                Extension = ".scs",
                Name = "Server-Based Certificate Validation Protocol – Validation Response",
                MimeType = "application/scvp-cv-response",
            },
            new ApacheMime()
            {
                Extension = ".sdp",
                Name = "Session Description Protocol",
                MimeType = "application/sdp",
            },
            new ApacheMime()
            {
                Extension = ".etx",
                Name = "Setext",
                MimeType = "text/x-setext",
            },
            new ApacheMime()
            {
                Extension = ".movie",
                Name = "SGI Movie",
                MimeType = "video/x-sgi-movie",
            },
            new ApacheMime()
            {
                Extension = ".ifm",
                Name = "Shana Informed Filler",
                MimeType = "application/vnd.shana.informed.formdata",
            },
            new ApacheMime()
            {
                Extension = ".itp",
                Name = "Shana Informed Filler",
                MimeType = "application/vnd.shana.informed.formtemplate",
            },
            new ApacheMime()
            {
                Extension = ".iif",
                Name = "Shana Informed Filler",
                MimeType = "application/vnd.shana.informed.interchange",
            },
            new ApacheMime()
            {
                Extension = ".ipk",
                Name = "Shana Informed Filler",
                MimeType = "application/vnd.shana.informed.package",
            },
            new ApacheMime()
            {
                Extension = ".tfi",
                Name = "Sharing Transaction Fraud Data",
                MimeType = "application/thraud+xml",
            },
            new ApacheMime()
            {
                Extension = ".shar",
                Name = "Shell Archive",
                MimeType = "application/x-shar",
            },
            new ApacheMime()
            {
                Extension = ".rgb",
                Name = "Silicon Graphics RGB Bitmap",
                MimeType = "image/x-rgb",
            },
            new ApacheMime()
            {
                Extension = ".slt",
                Name = "SimpleAnimeLite Player",
                MimeType = "application/vnd.epson.salt",
            },
            new ApacheMime()
            {
                Extension = ".aso",
                Name = "Simply Accounting",
                MimeType = "application/vnd.accpac.simply.aso",
            },
            new ApacheMime()
            {
                Extension = ".imp",
                Name = "Simply Accounting – Data Import",
                MimeType = "application/vnd.accpac.simply.imp",
            },
            new ApacheMime()
            {
                Extension = ".twd",
                Name = "SimTech MindMapper",
                MimeType = "application/vnd.simtech-mindmapper",
            },
            new ApacheMime()
            {
                Extension = ".csp",
                Name = "Sixth Floor Media – CommonSpace",
                MimeType = "application/vnd.commonspace",
            },
            new ApacheMime()
            {
                Extension = ".saf",
                Name = "SMAF Audio",
                MimeType = "application/vnd.yamaha.smaf-audio",
            },
            new ApacheMime()
            {
                Extension = ".mmf",
                Name = "SMAF File",
                MimeType = "application/vnd.smaf",
            },
            new ApacheMime()
            {
                Extension = ".spf",
                Name = "SMAF Phrase",
                MimeType = "application/vnd.yamaha.smaf-phrase",
            },
            new ApacheMime()
            {
                Extension = ".teacher",
                Name = "SMART Technologies Apps",
                MimeType = "application/vnd.smart.teacher",
            },
            new ApacheMime()
            {
                Extension = ".svd",
                Name = "SourceView Document",
                MimeType = "application/vnd.svd",
            },
            new ApacheMime()
            {
                Extension = ".rq",
                Name = "SPARQL – Query",
                MimeType = "application/sparql-query",
            },
            new ApacheMime()
            {
                Extension = ".srx",
                Name = "SPARQL – Results",
                MimeType = "application/sparql-results+xml",
            },
            new ApacheMime()
            {
                Extension = ".gram",
                Name = "Speech Recognition Grammar Specification",
                MimeType = "application/srgs",
            },
            new ApacheMime()
            {
                Extension = ".grxml",
                Name = "Speech Recognition Grammar Specification – XML",
                MimeType = "application/srgs+xml",
            },
            new ApacheMime()
            {
                Extension = ".ssml",
                Name = "Speech Synthesis Markup Language",
                MimeType = "application/ssml+xml",
            },
            new ApacheMime()
            {
                Extension = ".skp",
                Name = "SSEYO Koan Play File",
                MimeType = "application/vnd.koan",
            },
            new ApacheMime()
            {
                Extension = ".sgml",
                Name = "Standard Generalized Markup Language (SGML)",
                MimeType = "text/sgml",
            },
            new ApacheMime()
            {
                Extension = ".sdc",
                Name = "StarOffice – Calc",
                MimeType = "application/vnd.stardivision.calc",
            },
            new ApacheMime()
            {
                Extension = ".sda",
                Name = "StarOffice – Draw",
                MimeType = "application/vnd.stardivision.draw",
            },
            new ApacheMime()
            {
                Extension = ".sdd",
                Name = "StarOffice – Impress",
                MimeType = "application/vnd.stardivision.impress",
            },
            new ApacheMime()
            {
                Extension = ".smf",
                Name = "StarOffice – Math",
                MimeType = "application/vnd.stardivision.math",
            },
            new ApacheMime()
            {
                Extension = ".sdw",
                Name = "StarOffice – Writer",
                MimeType = "application/vnd.stardivision.writer",
            },
            new ApacheMime()
            {
                Extension = ".sgl",
                Name = "StarOffice – Writer (Global)",
                MimeType = "application/vnd.stardivision.writer-global",
            },
            new ApacheMime()
            {
                Extension = ".sm",
                Name = "StepMania",
                MimeType = "application/vnd.stepmania.stepchart",
            },
            new ApacheMime()
            {
                Extension = ".sit",
                Name = "Stuffit Archive",
                MimeType = "application/x-stuffit",
            },
            new ApacheMime()
            {
                Extension = ".sitx",
                Name = "Stuffit Archive",
                MimeType = "application/x-stuffitx",
            },
            new ApacheMime()
            {
                Extension = ".sdkm",
                Name = "SudokuMagic",
                MimeType = "application/vnd.solent.sdkm+xml",
            },
            new ApacheMime()
            {
                Extension = ".xo",
                Name = "Sugar Linux Application Bundle",
                MimeType = "application/vnd.olpc-sugar",
            },
            new ApacheMime()
            {
                Extension = ".au",
                Name = "Sun Audio – Au file format",
                MimeType = "audio/basic",
            },
            new ApacheMime()
            {
                Extension = ".wqd",
                Name = "SundaHus WQ",
                MimeType = "application/vnd.wqd",
            },
            new ApacheMime()
            {
                Extension = ".sis",
                Name = "Symbian Install Package",
                MimeType = "application/vnd.symbian.install",
            },
            new ApacheMime()
            {
                Extension = ".smi",
                Name = "Synchronized Multimedia Integration Language",
                MimeType = "application/smil+xml",
            },
            new ApacheMime()
            {
                Extension = ".xsm",
                Name = "SyncML",
                MimeType = "application/vnd.syncml+xml",
            },
            new ApacheMime()
            {
                Extension = ".bdm",
                Name = "SyncML – Device Management",
                MimeType = "application/vnd.syncml.dm+wbxml",
            },
            new ApacheMime()
            {
                Extension = ".xdm",
                Name = "SyncML – Device Management",
                MimeType = "application/vnd.syncml.dm+xml",
            },
            new ApacheMime()
            {
                Extension = ".sv4cpio",
                Name = "System V Release 4 CPIO Archive",
                MimeType = "application/x-sv4cpio",
            },
            new ApacheMime()
            {
                Extension = ".sv4crc",
                Name = "System V Release 4 CPIO Checksum Data",
                MimeType = "application/x-sv4crc",
            },
            new ApacheMime()
            {
                Extension = ".sbml",
                Name = "Systems Biology Markup Language",
                MimeType = "application/sbml+xml",
            },
            new ApacheMime()
            {
                Extension = ".tsv",
                Name = "Tab Seperated Values",
                MimeType = "text/tab-separated-values",
            },
            new ApacheMime()
            {
                Extension = ".tiff",
                Name = "Tagged Image File Format",
                MimeType = "image/tiff",
            },
            new ApacheMime()
            {
                Extension = ".tao",
                Name = "Tao Intent",
                MimeType = "application/vnd.tao.intent-module-archive",
            },
            new ApacheMime()
            {
                Extension = ".tar",
                Name = "Tar File (Tape Archive)",
                MimeType = "application/x-tar",
            },
            new ApacheMime()
            {
                Extension = ".tcl",
                Name = "Tcl Script",
                MimeType = "application/x-tcl",
            },
            new ApacheMime()
            {
                Extension = ".tex",
                Name = "TeX",
                MimeType = "application/x-tex",
            },
            new ApacheMime()
            {
                Extension = ".tfm",
                Name = "TeX Font Metric",
                MimeType = "application/x-tex-tfm",
            },
            new ApacheMime()
            {
                Extension = ".tei",
                Name = "Text Encoding and Interchange",
                MimeType = "application/tei+xml",
            },
            new ApacheMime()
            {
                Extension = ".txt",
                Name = "Text File",
                MimeType = "text/plain",
            },
            new ApacheMime()
            {
                Extension = ".dxp",
                Name = "TIBCO Spotfire",
                MimeType = "application/vnd.spotfire.dxp",
            },
            new ApacheMime()
            {
                Extension = ".sfs",
                Name = "TIBCO Spotfire",
                MimeType = "application/vnd.spotfire.sfs",
            },
            new ApacheMime()
            {
                Extension = ".tsd",
                Name = "Time Stamped Data Envelope",
                MimeType = "application/timestamped-data",
            },
            new ApacheMime()
            {
                Extension = ".tpt",
                Name = "TRI Systems Config",
                MimeType = "application/vnd.trid.tpt",
            },
            new ApacheMime()
            {
                Extension = ".mxs",
                Name = "Triscape Map Explorer",
                MimeType = "application/vnd.triscape.mxs",
            },
            new ApacheMime()
            {
                Extension = ".t",
                Name = "troff",
                MimeType = "text/troff",
            },
            new ApacheMime()
            {
                Extension = ".tra",
                Name = "True BASIC",
                MimeType = "application/vnd.trueapp",
            },
            new ApacheMime()
            {
                Extension = ".ttf",
                Name = "TrueType Font",
                MimeType = "application/x-font-ttf",
            },
            new ApacheMime()
            {
                Extension = ".ttl",
                Name = "Turtle (Terse RDF Triple Language)",
                MimeType = "text/turtle",
            },
            new ApacheMime()
            {
                Extension = ".umj",
                Name = "UMAJIN",
                MimeType = "application/vnd.umajin",
            },
            new ApacheMime()
            {
                Extension = ".uoml",
                Name = "Unique Object Markup Language",
                MimeType = "application/vnd.uoml+xml",
            },
            new ApacheMime()
            {
                Extension = ".unityweb",
                Name = "Unity 3d",
                MimeType = "application/vnd.unity",
            },
            new ApacheMime()
            {
                Extension = ".ufd",
                Name = "Universal Forms Description Language",
                MimeType = "application/vnd.ufdl",
            },
            new ApacheMime()
            {
                Extension = ".uri",
                Name = "URI Resolution Services",
                MimeType = "text/uri-list",
            },
            new ApacheMime()
            {
                Extension = ".utz",
                Name = "User Interface Quartz – Theme (Symbian)",
                MimeType = "application/vnd.uiq.theme",
            },
            new ApacheMime()
            {
                Extension = ".ustar",
                Name = "Ustar (Uniform Standard Tape Archive)",
                MimeType = "application/x-ustar",
            },
            new ApacheMime()
            {
                Extension = ".uu",
                Name = "UUEncode",
                MimeType = "text/x-uuencode",
            },
            new ApacheMime()
            {
                Extension = ".vcs",
                Name = "vCalendar",
                MimeType = "text/x-vcalendar",
            },
            new ApacheMime()
            {
                Extension = ".vcf",
                Name = "vCard",
                MimeType = "text/x-vcard",
            },
            new ApacheMime()
            {
                Extension = ".vcd",
                Name = "Video CD",
                MimeType = "application/x-cdlink",
            },
            new ApacheMime()
            {
                Extension = ".vsf",
                Name = "Viewport+",
                MimeType = "application/vnd.vsf",
            },
            new ApacheMime()
            {
                Extension = ".wrl",
                Name = "Virtual Reality Modeling Language",
                MimeType = "model/vrml",
            },
            new ApacheMime()
            {
                Extension = ".vcx",
                Name = "VirtualCatalog",
                MimeType = "application/vnd.vcx",
            },
            new ApacheMime()
            {
                Extension = ".mts",
                Name = "Virtue MTS",
                MimeType = "model/vnd.mts",
            },
            new ApacheMime()
            {
                Extension = ".vtu",
                Name = "Virtue VTU",
                MimeType = "model/vnd.vtu",
            },
            new ApacheMime()
            {
                Extension = ".vis",
                Name = "Visionary",
                MimeType = "application/vnd.visionary",
            },
            new ApacheMime()
            {
                Extension = ".viv",
                Name = "Vivo",
                MimeType = "video/vnd.vivo",
            },
            new ApacheMime()
            {
                Extension = ".ccxml",
                Name = "Voice Browser Call Control",
                MimeType = "application/ccxml+xml,",
            },
            new ApacheMime()
            {
                Extension = ".vxml",
                Name = "VoiceXML",
                MimeType = "application/voicexml+xml",
            },
            new ApacheMime()
            {
                Extension = ".src",
                Name = "WAIS Source",
                MimeType = "application/x-wais-source",
            },
            new ApacheMime()
            {
                Extension = ".wbxml",
                Name = "WAP Binary XML (WBXML)",
                MimeType = "application/vnd.wap.wbxml",
            },
            new ApacheMime()
            {
                Extension = ".wbmp",
                Name = "WAP Bitamp (WBMP)",
                MimeType = "image/vnd.wap.wbmp",
            },
            new ApacheMime()
            {
                Extension = ".wav",
                Name = "Waveform Audio File Format (WAV)",
                MimeType = "audio/x-wav",
            },
            new ApacheMime()
            {
                Extension = ".davmount",
                Name = "Web Distributed Authoring and Versioning",
                MimeType = "application/davmount+xml",
            },
            new ApacheMime()
            {
                Extension = ".woff",
                Name = "Web Open Font Format",
                MimeType = "application/x-font-woff",
            },
            new ApacheMime()
            {
                Extension = ".wspolicy",
                Name = "Web Services Policy",
                MimeType = "application/wspolicy+xml",
            },
            new ApacheMime()
            {
                Extension = ".webp",
                Name = "WebP Image",
                MimeType = "image/webp",
            },
            new ApacheMime()
            {
                Extension = ".wtb",
                Name = "WebTurbo",
                MimeType = "application/vnd.webturbo",
            },
            new ApacheMime()
            {
                Extension = ".wgt",
                Name = "Widget Packaging and XML Configuration",
                MimeType = "application/widget",
            },
            new ApacheMime()
            {
                Extension = ".hlp",
                Name = "WinHelp",
                MimeType = "application/winhlp",
            },
            new ApacheMime()
            {
                Extension = ".wml",
                Name = "Wireless Markup Language (WML)",
                MimeType = "text/vnd.wap.wml",
            },
            new ApacheMime()
            {
                Extension = ".wmls",
                Name = "Wireless Markup Language Script (WMLScript)",
                MimeType = "text/vnd.wap.wmlscript",
            },
            new ApacheMime()
            {
                Extension = ".wmlsc",
                Name = "WMLScript",
                MimeType = "application/vnd.wap.wmlscriptc",
            },
            new ApacheMime()
            {
                Extension = ".wpd",
                Name = "Wordperfect",
                MimeType = "application/vnd.wordperfect",
            },
            new ApacheMime()
            {
                Extension = ".stf",
                Name = "Worldtalk",
                MimeType = "application/vnd.wt.stf",
            },
            new ApacheMime()
            {
                Extension = ".wsdl",
                Name = "WSDL – Web Services Description Language",
                MimeType = "application/wsdl+xml",
            },
            new ApacheMime()
            {
                Extension = ".xbm",
                Name = "X BitMap",
                MimeType = "image/x-xbitmap",
            },
            new ApacheMime()
            {
                Extension = ".xpm",
                Name = "X PixMap",
                MimeType = "image/x-xpixmap",
            },
            new ApacheMime()
            {
                Extension = ".xwd",
                Name = "X Window Dump",
                MimeType = "image/x-xwindowdump",
            },
            new ApacheMime()
            {
                Extension = ".der",
                Name = "X.509 Certificate",
                MimeType = "application/x-x509-ca-cert",
            },
            new ApacheMime()
            {
                Extension = ".fig",
                Name = "Xfig",
                MimeType = "application/x-xfig",
            },
            new ApacheMime()
            {
                Extension = ".xhtml",
                Name = "XHTML – The Extensible HyperText Markup Language",
                MimeType = "application/xhtml+xml",
            },
            new ApacheMime()
            {
                Extension = ".xml",
                Name = "XML – Extensible Markup Language",
                MimeType = "application/xml",
            },
            new ApacheMime()
            {
                Extension = ".xdf",
                Name = "XML Configuration Access Protocol – XCAP Diff",
                MimeType = "application/xcap-diff+xml",
            },
            new ApacheMime()
            {
                Extension = ".xenc",
                Name = "XML Encryption Syntax and Processing",
                MimeType = "application/xenc+xml",
            },
            new ApacheMime()
            {
                Extension = ".xer",
                Name = "XML Patch Framework",
                MimeType = "application/patch-ops-error+xml",
            },
            new ApacheMime()
            {
                Extension = ".rl",
                Name = "XML Resource Lists",
                MimeType = "application/resource-lists+xml",
            },
            new ApacheMime()
            {
                Extension = ".rs",
                Name = "XML Resource Lists",
                MimeType = "application/rls-services+xml",
            },
            new ApacheMime()
            {
                Extension = ".rld",
                Name = "XML Resource Lists Diff",
                MimeType = "application/resource-lists-diff+xml",
            },
            new ApacheMime()
            {
                Extension = ".xslt",
                Name = "XML Transformations",
                MimeType = "application/xslt+xml",
            },
            new ApacheMime()
            {
                Extension = ".xop",
                Name = "XML-Binary Optimized Packaging",
                MimeType = "application/xop+xml",
            },
            new ApacheMime()
            {
                Extension = ".xpi",
                Name = "XPInstall – Mozilla",
                MimeType = "application/x-xpinstall",
            },
            new ApacheMime()
            {
                Extension = ".xspf",
                Name = "XSPF – XML Shareable Playlist Format",
                MimeType = "application/xspf+xml",
            },
            new ApacheMime()
            {
                Extension = ".xul",
                Name = "XUL – XML User Interface Language",
                MimeType = "application/vnd.mozilla.xul+xml",
            },
            new ApacheMime()
            {
                Extension = ".xyz",
                Name = "XYZ File Format",
                MimeType = "chemical/x-xyz",
            },
            new ApacheMime()
            {
                Extension = ".yaml",
                Name = "YAML Ain’t Markup Language / Yet Another Markup Language",
                MimeType = "text/yaml",
            },
            new ApacheMime()
            {
                Extension = ".yang",
                Name = "YANG Data Modeling Language",
                MimeType = "application/yang",
            },
            new ApacheMime()
            {
                Extension = ".yin",
                Name = "YIN (YANG – XML)",
                MimeType = "application/yin+xml",
            },
            new ApacheMime()
            {
                Extension = ".zir",
                Name = "Z.U.L. Geometry",
                MimeType = "application/vnd.zul",
            },
            new ApacheMime()
            {
                Extension = ".zip",
                Name = "Zip Archive",
                MimeType = "application/zip",
            },
            new ApacheMime()
            {
                Extension = ".zmm",
                Name = "ZVUE Media Manager",
                MimeType = "application/vnd.handheld-entertainment+xml",
            },
            new ApacheMime()
            {
                Extension = ".zaz",
                Name = "Zzazz Deck",
                MimeType = "application/vnd.zzazz.deck+xml",
            }

        };
}