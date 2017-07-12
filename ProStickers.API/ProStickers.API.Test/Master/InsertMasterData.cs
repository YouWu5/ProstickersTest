using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Microsoft.WindowsAzure.Storage.Table;
using OfficeOpenXml;
using ProStickers.CloudDAL;
using ProStickers.CloudDAL.Entity.Master;
using ProStickers.ViewModel.Core;

namespace ProStickers.API.Test.Master
{
    public class InsertMasterData
    {
        public static CloudTable masterDataTable;

        static InsertMasterData()
        {
            masterDataTable = Utility.GetStorageTable("CustomerAppointment");
        }

        [Fact]
        public void InsertStateData()
        {
            var fi = new FileInfo(@"D:\Jain.xlsx");
            byte[] bytes = File.ReadAllBytes(fi.ToString());

            //    MemoryStream stream = new MemoryStream(bytes);
            //    ExcelPackage excel = new ExcelPackage(stream);
            //    var workSheet = excel.Workbook.Worksheets.First();
            //    IEnumerable<ExcelData> list = workSheet.ConvertStateSheetToObjects<ExcelData>();

            //    List<CustomerAppointmentEntity> stateEntityList = new List<CustomerAppointmentEntity>();
            //    foreach (var item in list)
            //    {
            //        //if (item.PartitionKey != null)
            //        //{
            //        CustomerAppointmentEntity stateEntity = new CustomerAppointmentEntity();

            //        stateEntity.PartitionKey = item.PartitionKey.PadLeft(8, '0');
            //        stateEntity.RowKey = (DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks).ToString("d19");
            //        stateEntity.StartTime = item.StartTime;
            //        stateEntity.StatusID = item.StatusID;
            //        stateEntity.UpdatedBy = item.UpdatedBy == "1" ? item.CreatedBy.PadLeft(8, '0') : item.UpdatedBy;
            //        stateEntity.UpdatedTS = item.UpdatedTS;
            //        stateEntity.AppointmentDateTime = item.AppointmentDateTime;
            //        stateEntity.AppointmentNumber = item.AppointmentNumber;
            //        stateEntity.AppointmentStatus = item.AppointmentStatus;
            //        stateEntity.AppointmentStatusID = item.AppointmentStatusID;
            //        stateEntity.CreatedBy = item.CreatedBy == "1" ? item.CreatedBy.PadLeft(8, '0') : item.CreatedBy;
            //        stateEntity.CancellationReason = item.CancellationReason;
            //        stateEntity.CreatedTS = item.CreatedTS;
            //        stateEntity.Date = Convert.ToDateTime(item.Date).ToString("yyyy-MM-dd");
            //        stateEntity.DesignNumber = item.DesignNumber;
            //        stateEntity.EndTime = item.EndTime;
            //        stateEntity.ImageStatusID = item.ImageStatusID;
            //        stateEntity.RequestDate = Convert.ToDateTime(item.RequestDate).ToString("yyyy-MM-dd");
            //        stateEntity.RequestDateTime = item.RequestDateTime;
            //        stateEntity.UserID = item.UserID;
            //        stateEntity.UserName = item.UserName;
            //        // }
            //        TableOperation insert = TableOperation.Insert(stateEntity);
            //        masterDataTable.ExecuteAsync(insert);
            //    }
            //    //if (stateEntityList != null && stateEntityList.Count > 0)
            //    //{

            //    //    InsertMasterDataEntity insertMasterEntity = new InsertMasterDataEntity();
            //    //    //insertMasterEntity.MasterDataJSON = Newtonsoft.Json.JsonConvert.SerializeObject(stateEntityList);
            //    //    //if (insertMasterEntity.MasterDataJSON != null)
            //    //    //{
            //    //    //    insertMasterEntity.PartitionKey = "State";
            //    //    //    insertMasterEntity.RowKey = "State";
            //    //      TableOperation insert = TableOperation.Insert(insertMasterEntity);
            //    //        masterDataTable.ExecuteAsync(insert);
            //    //    }
        }

        [Fact]
        public void InsertFileExtension()
        {
            List<ListItem> list = new List<ListItem>();
            list.Add(new ListItem { Text = "aif", Value = "audio/x-aiff" });
            list.Add(new ListItem { Text = "aifc", Value = "audio/x-aiff" });
            list.Add(new ListItem { Text = "aiff", Value = "audio/x-aiff" });
            list.Add(new ListItem { Text = "asc", Value = "text/plain" });
            list.Add(new ListItem { Text = "atom", Value = "application/atom+xml" });
            list.Add(new ListItem { Text = "au", Value = "audio/basic" });
            list.Add(new ListItem { Text = "avi", Value = "video/x-msvideo" });
            list.Add(new ListItem { Text = "bcpio", Value = "application/x-bcpio" });
            list.Add(new ListItem { Text = "bin", Value = "application/octet-stream" });
            list.Add(new ListItem { Text = "bmp", Value = "image/bmp" });
            list.Add(new ListItem { Text = "cdf", Value = "application/x-netcdf" });
            list.Add(new ListItem { Text = "cgm", Value = "image/cgm" });
            list.Add(new ListItem { Text = "class", Value = "application/octet-stream" });
            list.Add(new ListItem { Text = "cpio", Value = "application/x-cpio" });
            list.Add(new ListItem { Text = "cpt", Value = "application/mac-compactpro" });
            list.Add(new ListItem { Text = "csh", Value = "application/x-csh" });
            list.Add(new ListItem { Text = "css", Value = "text/css" });
            list.Add(new ListItem { Text = "dcr", Value = "application/x-director" });
            list.Add(new ListItem { Text = "dif", Value = "video/x-dv" });
            list.Add(new ListItem { Text = "dir", Value = "application/x-director" });
            list.Add(new ListItem { Text = "djv", Value = "image/vnd.djvu" });
            list.Add(new ListItem { Text = "djvu", Value = "image/vnd.djvu" });
            list.Add(new ListItem { Text = "dll", Value = "application/octet-stream" });
            list.Add(new ListItem { Text = "dmg", Value = "application/octet-stream" });
            list.Add(new ListItem { Text = "dms", Value = "application/octet-stream" });
            list.Add(new ListItem { Text = "doc", Value = "application/msword" });
            list.Add(new ListItem { Text = "docx", Value = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" });
            list.Add(new ListItem { Text = "dotx", Value = "application/vnd.openxmlformats-officedocument.wordprocessingml.template" });
            list.Add(new ListItem { Text = "docm", Value = "application/vnd.ms-word.document.macroEnabled.12" });
            list.Add(new ListItem { Text = "dotm", Value = "application/vnd.ms-word.template.macroEnabled.12" });
            list.Add(new ListItem { Text = "dtd", Value = "application/xml-dtd" });
            list.Add(new ListItem { Text = "dv", Value = "video/x-dv" });
            list.Add(new ListItem { Text = "dvi", Value = "application/x-dvi" });
            list.Add(new ListItem { Text = "dxr", Value = "application/x-director" });
            list.Add(new ListItem { Text = "eps", Value = "application/postscript" });
            list.Add(new ListItem { Text = "etx", Value = "text/x-setext" });
            list.Add(new ListItem { Text = "exe", Value = "application/octet-stream" });
            list.Add(new ListItem { Text = "ez", Value = "application/andrew-inset" });
            list.Add(new ListItem { Text = "gif", Value = "image/gif" });
            list.Add(new ListItem { Text = "gram", Value = "application/srgs" });
            list.Add(new ListItem { Text = "grxml", Value = "application/srgs+xml" });
            list.Add(new ListItem { Text = "gtar", Value = "application/x-gtar" });
            list.Add(new ListItem { Text = "hdf", Value = "application/x-hdf" });
            list.Add(new ListItem { Text = "hqx", Value = "application/mac-binhex40" });
            list.Add(new ListItem { Text = "htm", Value = "text/html" });
            list.Add(new ListItem { Text = "html", Value = "text/html" });
            list.Add(new ListItem { Text = "ice", Value = "x-conference/x-cooltalk" });
            list.Add(new ListItem { Text = "ico", Value = "image/x-icon" });
            list.Add(new ListItem { Text = "ics", Value = "text/calendar" });
            list.Add(new ListItem { Text = "ief", Value = "image/ief" });
            list.Add(new ListItem { Text = "ifb", Value = "text/calendar" });
            list.Add(new ListItem { Text = "iges", Value = "model/iges" });
            list.Add(new ListItem { Text = "igs", Value = "model/iges" });
            list.Add(new ListItem { Text = "jnlp", Value = "application/x-java-jnlp-file" });
            list.Add(new ListItem { Text = "jp2", Value = "image/jp2" });
            list.Add(new ListItem { Text = "jpe", Value = "image/jpeg" });
            list.Add(new ListItem { Text = "jpeg", Value = "image/jpeg" });
            list.Add(new ListItem { Text = "jpg", Value = "image/jpeg" });
            list.Add(new ListItem { Text = "js", Value = "application/x-javascript" });
            list.Add(new ListItem { Text = "kar", Value = "audio/midi" });
            list.Add(new ListItem { Text = "latex", Value = "application/x-latex" });
            list.Add(new ListItem { Text = "lha", Value = "application/octet-stream" });
            list.Add(new ListItem { Text = "lzh", Value = "application/octet-stream" });
            list.Add(new ListItem { Text = "m3u", Value = "audio/x-mpegurl" });
            list.Add(new ListItem { Text = "m4a", Value = "audio/mp4a-latm" });
            list.Add(new ListItem { Text = "m4b", Value = "audio/mp4a-latm" });
            list.Add(new ListItem { Text = "m4p", Value = "audio/mp4a-latm" });
            list.Add(new ListItem { Text = "m4u", Value = "video/vnd.mpegurl" });
            list.Add(new ListItem { Text = "m4v", Value = "video/x-m4v" });
            list.Add(new ListItem { Text = "mac", Value = "image/x-macpaint" });
            list.Add(new ListItem { Text = "man", Value = "application/x-troff-man" });
            list.Add(new ListItem { Text = "mathml", Value = "application/mathml+xml" });
            list.Add(new ListItem { Text = "me", Value = "application/x-troff-me" });
            list.Add(new ListItem { Text = "mesh", Value = "model/mesh" });
            list.Add(new ListItem { Text = "mid", Value = "audio/midi" });
            list.Add(new ListItem { Text = "midi", Value = "audio/midi" });
            list.Add(new ListItem { Text = "mif", Value = "application/vnd.mif" });
            list.Add(new ListItem { Text = "mov", Value = "video/quicktime" });
            list.Add(new ListItem { Text = "movie", Value = "video/x-sgi-movie" });
            list.Add(new ListItem { Text = "mp2", Value = "audio/mpeg" });
            list.Add(new ListItem { Text = "mp3", Value = "audio/mpeg" });
            list.Add(new ListItem { Text = "mp4", Value = "video/mp4" });
            list.Add(new ListItem { Text = "mpe", Value = "video/mpeg" });
            list.Add(new ListItem { Text = "mpeg", Value = "video/mpeg" });
            list.Add(new ListItem { Text = "mpg", Value = "video/mpeg" });
            list.Add(new ListItem { Text = "mpga", Value = "audio/mpeg" });
            list.Add(new ListItem { Text = "ms", Value = "application/x-troff-ms" });
            list.Add(new ListItem { Text = "msh", Value = "model/mesh" });
            list.Add(new ListItem { Text = "mxu", Value = "video/vnd.mpegurl" });
            list.Add(new ListItem { Text = "nc", Value = "application/x-netcdf" });
            list.Add(new ListItem { Text = "oda", Value = "application/oda" });
            list.Add(new ListItem { Text = "ogg", Value = "application/ogg" });
            list.Add(new ListItem { Text = "pbm", Value = "image/x-portable-bitmap" });
            list.Add(new ListItem { Text = "pct", Value = "image/pict" });
            list.Add(new ListItem { Text = "pdb", Value = "chemical/x-pdb" });
            list.Add(new ListItem { Text = "pdf", Value = "application/pdf" });
            list.Add(new ListItem { Text = "pgm", Value = "image/x-portable-graymap" });
            list.Add(new ListItem { Text = "pgn", Value = "application/x-chess-pgn" });
            list.Add(new ListItem { Text = "pic", Value = "image/pict" });
            list.Add(new ListItem { Text = "pict", Value = "image/pict" });
            list.Add(new ListItem { Text = "png", Value = "image/png" });
            list.Add(new ListItem { Text = "pnm", Value = "image/x-portable-anymap" });
            list.Add(new ListItem { Text = "pnt", Value = "image/x-macpaint" });
            list.Add(new ListItem { Text = "pntg", Value = "image/x-macpaint" });
            list.Add(new ListItem { Text = "ppm", Value = "image/x-portable-pixmap" });
            list.Add(new ListItem { Text = "ppt", Value = "application/vnd.ms-powerpoint" });
            list.Add(new ListItem { Text = "pptx", Value = "application/vnd.openxmlformats-officedocument.presentationml.presentation" });
            list.Add(new ListItem { Text = "potx", Value = "application/vnd.openxmlformats-officedocument.presentationml.template" });
            list.Add(new ListItem { Text = "ppsx", Value = "application/vnd.openxmlformats-officedocument.presentationml.slideshow" });
            list.Add(new ListItem { Text = "ppam", Value = "application/vnd.ms-powerpoint.addin.macroEnabled.12" });
            list.Add(new ListItem { Text = "pptm", Value = "application/vnd.ms-powerpoint.presentation.macroEnabled.12" });
            list.Add(new ListItem { Text = "potm", Value = "application/vnd.ms-powerpoint.template.macroEnabled.12" });
            list.Add(new ListItem { Text = "ppsm", Value = "application/vnd.ms-powerpoint.slideshow.macroEnabled.12" });
            list.Add(new ListItem { Text = "ps", Value = "application/postscript" });
            list.Add(new ListItem { Text = "qt", Value = "video/quicktime" });
            list.Add(new ListItem { Text = "qti", Value = "image/x-quicktime" });
            list.Add(new ListItem { Text = "qtif", Value = "image/x-quicktime" });
            list.Add(new ListItem { Text = "ra", Value = "audio/x-pn-realaudio" });
            list.Add(new ListItem { Text = "ram", Value = "audio/x-pn-realaudio" });
            list.Add(new ListItem { Text = "ras", Value = "image/x-cmu-raster" });
            list.Add(new ListItem { Text = "rdf", Value = "application/rdf+xml" });
            list.Add(new ListItem { Text = "rgb", Value = "image/x-rgb" });
            list.Add(new ListItem { Text = "rm", Value = "application/vnd.rn-realmedia" });
            list.Add(new ListItem { Text = "roff", Value = "application/x-troff" });
            list.Add(new ListItem { Text = "rtf", Value = "text/rtf" });
            list.Add(new ListItem { Text = "rtx", Value = "text/richtext" });
            list.Add(new ListItem { Text = "sgm", Value = "text/sgml" });
            list.Add(new ListItem { Text = "sgml", Value = "text/sgml" });
            list.Add(new ListItem { Text = "sh", Value = "application/x-sh" });
            list.Add(new ListItem { Text = "shar", Value = "application/x-shar" });
            list.Add(new ListItem { Text = "silo", Value = "model/mesh" });
            list.Add(new ListItem { Text = "sit", Value = "application/x-stuffit" });
            list.Add(new ListItem { Text = "skd", Value = "application/x-koan" });
            list.Add(new ListItem { Text = "skm", Value = "application/x-koan" });
            list.Add(new ListItem { Text = "skp", Value = "application/x-koan" });
            list.Add(new ListItem { Text = "skt", Value = "application/x-koan" });
            list.Add(new ListItem { Text = "smi", Value = "application/smil" });
            list.Add(new ListItem { Text = "smil", Value = "application/smil" });
            list.Add(new ListItem { Text = "snd", Value = "audio/basic" });
            list.Add(new ListItem { Text = "so", Value = "application/octet-stream" });
            list.Add(new ListItem { Text = "spl", Value = "application/x-futuresplash" });
            list.Add(new ListItem { Text = "src", Value = "application/x-wais-source" });
            list.Add(new ListItem { Text = "sv4cpio", Value = "application/x-sv4cpio" });
            list.Add(new ListItem { Text = "sv4crc", Value = "application/x-sv4crc" });
            list.Add(new ListItem { Text = "svg", Value = "image/svg+xml" });
            list.Add(new ListItem { Text = "swf", Value = "application/x-shockwave-flash" });
            list.Add(new ListItem { Text = "t", Value = "application/x-troff" });
            list.Add(new ListItem { Text = "tar", Value = "application/x-tar" });
            list.Add(new ListItem { Text = "tcl", Value = "application/x-tcl" });
            list.Add(new ListItem { Text = "tex", Value = "application/x-tex" });
            list.Add(new ListItem { Text = "texi", Value = "application/x-texinfo" });
            list.Add(new ListItem { Text = "texinfo", Value = "application/x-texinfo" });
            list.Add(new ListItem { Text = "tif", Value = "image/tiff" });
            list.Add(new ListItem { Text = "tiff", Value = "image/tiff" });
            list.Add(new ListItem { Text = "tr", Value = "application/x-troff" });
            list.Add(new ListItem { Text = "tsv", Value = "text/tab-separated-values" });
            list.Add(new ListItem { Text = "txt", Value = "text/plain" });
            list.Add(new ListItem { Text = "ustar", Value = "application/x-ustar" });
            list.Add(new ListItem { Text = "vcd", Value = "application/x-cdlink" });
            list.Add(new ListItem { Text = "vrml", Value = "model/vrml" });
            list.Add(new ListItem { Text = "vxml", Value = "application/voicexml+xml" });
            list.Add(new ListItem { Text = "wav", Value = "audio/x-wav" });
            list.Add(new ListItem { Text = "wbmp", Value = "image/vnd.wap.wbmp" });
            list.Add(new ListItem { Text = "wbmxl", Value = "application/vnd.wap.wbxml" });
            list.Add(new ListItem { Text = "wml", Value = "text/vnd.wap.wml" });
            list.Add(new ListItem { Text = "wmlc", Value = "application/vnd.wap.wmlc" });
            list.Add(new ListItem { Text = "wmls", Value = "text/vnd.wap.wmlscript" });
            list.Add(new ListItem { Text = "wmlsc", Value = "application/vnd.wap.wmlscriptc" });
            list.Add(new ListItem { Text = "wrl", Value = "model/vrml" });
            list.Add(new ListItem { Text = "xbm", Value = "image/x-xbitmap" });
            list.Add(new ListItem { Text = "xht", Value = "application/xhtml+xml" });
            list.Add(new ListItem { Text = "xhtml", Value = "application/xhtml+xml" });
            list.Add(new ListItem { Text = "xls", Value = "application/vnd.ms-excel" });
            list.Add(new ListItem { Text = "xml", Value = "application/xml" });
            list.Add(new ListItem { Text = "xpm", Value = "image/x-xpixmap" });
            list.Add(new ListItem { Text = "xsl", Value = "application/xml" });
            list.Add(new ListItem { Text = "xlsx", Value = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            list.Add(new ListItem { Text = "xltx", Value = "application/vnd.openxmlformats-officedocument.spreadsheetml.template" });
            list.Add(new ListItem { Text = "xlsm", Value = "application/vnd.ms-excel.sheet.macroEnabled.12" });
            list.Add(new ListItem { Text = "xltm", Value = "application/vnd.ms-excel.template.macroEnabled.12" });
            list.Add(new ListItem { Text = "xlam", Value = "application/vnd.ms-excel.addin.macroEnabled.12" });
            list.Add(new ListItem { Text = "xlsb", Value = "application/vnd.ms-excel.sheet.binary.macroEnabled.12" });
            list.Add(new ListItem { Text = "xslt", Value = "application/xslt+xml" });
            list.Add(new ListItem { Text = "xul", Value = "application/vnd.mozilla.xul+xml" });
            list.Add(new ListItem { Text = "xwd", Value = "image/x-xwindowdump" });
            list.Add(new ListItem { Text = "xyz", Value = "chemical/x-xyz" });
            list.Add(new ListItem { Text = "zip", Value = "application/zip" });
            list.Add(new ListItem { Text = "psd", Value = "application/octet-stream" });
            list.Add(new ListItem { Text = "ai", Value = "application/postscript" });
            list.Add(new ListItem { Text = "dwg", Value = "application/acad" });
            list.Add(new ListItem { Text = "ma", Value = "application/mathematica" });
            list.Add(new ListItem { Text = "cdr", Value = "application/cdr" });

            InsertMasterDataEntity insertMasterEntity = new InsertMasterDataEntity();
            insertMasterEntity.MasterDataJSON = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            if (insertMasterEntity.MasterDataJSON != null)
            {
                insertMasterEntity.PartitionKey = "SupportedFileExtension";
                insertMasterEntity.RowKey = "SupportedFileExtension";
                TableOperation insert = TableOperation.InsertOrReplace(insertMasterEntity);
                masterDataTable.ExecuteAsync(insert);
            }
        }

        //    [Fact]
        //    public static void InsertCountryData()
        //    {
        //        var fi = new FileInfo(@"D:\FinalCountryList.xlsx");
        //        byte[] bytes = File.ReadAllBytes(fi.ToString());

        //        MemoryStream stream = new MemoryStream(bytes);
        //        ExcelPackage excel = new ExcelPackage(stream);
        //        var workSheet = excel.Workbook.Worksheets.First();
        //        IEnumerable<ExcelDataCountry> list = workSheet.ConvertCountrySheetToObjects<ExcelDataCountry>();

        //        List<CountryEntity> countryEntityList = new List<CountryEntity>();
        //        foreach (var item in list)
        //        {
        //            if (item.CountryID > 0)
        //            {
        //                CountryEntity cEntity = new CountryEntity();

        //                cEntity.CountryID = item.CountryID;
        //                cEntity.Name = item.Name;
        //                cEntity.PostalCodeRegax = item.PostalCodeRegax;
        //                countryEntityList.Add(cEntity);
        //            }
        //        }
        //        if (countryEntityList != null && countryEntityList.Count > 0)
        //        {

        //            InsertMasterDataEntity insertMasterEntity = new InsertMasterDataEntity();
        //            insertMasterEntity.MasterDataJSON = Newtonsoft.Json.JsonConvert.SerializeObject(countryEntityList);
        //            if (insertMasterEntity.MasterDataJSON != null)
        //            {
        //                insertMasterEntity.PartitionKey = "Country";
        //                insertMasterEntity.RowKey = "Country";
        //                TableOperation insert = TableOperation.InsertOrReplace(insertMasterEntity);
        //                masterDataTable.ExecuteAsync(insert);
        //            }
        //        }
        //    }

        //    [Fact]
        //    public static void InsertTimeSlotData()
        //    {
        //        var fi = new FileInfo(@"D:\Work\TimeSlotList.xlsx");
        //        byte[] bytes = File.ReadAllBytes(fi.ToString());

        //        MemoryStream stream = new MemoryStream(bytes);
        //        ExcelPackage excel = new ExcelPackage(stream);
        //        var workSheet = excel.Workbook.Worksheets.First();
        //        IEnumerable<ExcelTimeSlotData> list = workSheet.ConvertTimeSlotSheetToObjects<ExcelTimeSlotData>();

        //        List<TimeSlotEntity> timeSlotEntityList = new List<TimeSlotEntity>();
        //        foreach (var item in list)
        //        {
        //            TimeSlotEntity tsEntity = new TimeSlotEntity();

        //            tsEntity.TimeSlotID = Convert.ToInt32(item.TimeSlotID.ToString().PadLeft(2, '0'));
        //            tsEntity.Name = item.Name;
        //            tsEntity.StartTime = Utility.ConvertTime(Convert.ToDateTime(item.StartTime));
        //            tsEntity.EndTime = Utility.ConvertTime(Convert.ToDateTime(item.EndTime));
        //            timeSlotEntityList.Add(tsEntity);

        //        }
        //        if (timeSlotEntityList != null && timeSlotEntityList.Count > 0)
        //        {

        //            InsertMasterDataEntity insertMasterEntity = new InsertMasterDataEntity();
        //            insertMasterEntity.MasterDataJSON = Newtonsoft.Json.JsonConvert.SerializeObject(timeSlotEntityList);
        //            if (insertMasterEntity.MasterDataJSON != null)
        //            {
        //                insertMasterEntity.PartitionKey = "TimeSlot";
        //                insertMasterEntity.RowKey = "TimeSlot";
        //                TableOperation insert = TableOperation.Insert(insertMasterEntity);
        //                masterDataTable.ExecuteAsync(insert);
        //            }
        //        }
        //    }

        //    [Fact]
        //    public static void InsertMonthData()
        //    {
        //        List<ListItemTypes> list = new List<ListItemTypes>();
        //        list.Add(new ListItemTypes { Text = "January", Value = 1 });
        //        list.Add(new ListItemTypes { Text = "February", Value = 2 });
        //        list.Add(new ListItemTypes { Text = "March", Value = 3 });
        //        list.Add(new ListItemTypes { Text = "April", Value = 4 });
        //        list.Add(new ListItemTypes { Text = "May", Value = 5 });
        //        list.Add(new ListItemTypes { Text = "June", Value = 6 });
        //        list.Add(new ListItemTypes { Text = "July", Value = 7 });
        //        list.Add(new ListItemTypes { Text = "August", Value = 8 });
        //        list.Add(new ListItemTypes { Text = "September", Value = 9 });
        //        list.Add(new ListItemTypes { Text = "october", Value = 10 });
        //        list.Add(new ListItemTypes { Text = "November", Value = 11 });
        //        list.Add(new ListItemTypes { Text = "December", Value = 12 });
        //        if (list != null && list.Count() > 0)
        //        {
        //            List<MonthEntity> mlist = new List<MonthEntity>();
        //            foreach (var item in list)
        //            {
        //                MonthEntity mEntity = new MonthEntity();
        //                mEntity.MonthID = Convert.ToInt32(item.Value);
        //                mEntity.Name = item.Text;
        //                mlist.Add(mEntity);
        //            }
        //            if (mlist != null && mlist.Count() > 0)
        //            {
        //                InsertMasterDataEntity insertMasterEntity = new InsertMasterDataEntity();
        //                insertMasterEntity.MasterDataJSON = Newtonsoft.Json.JsonConvert.SerializeObject(mlist);
        //                if (insertMasterEntity.MasterDataJSON != null)
        //                {
        //                    insertMasterEntity.PartitionKey = "Month";
        //                    insertMasterEntity.RowKey = "Month";
        //                    TableOperation insert = TableOperation.Insert(insertMasterEntity);
        //                    masterDataTable.ExecuteAsync(insert);
        //                }
        //            }
        //        }
        //    }

        //    [Fact]
        //    public static void InsertYearData()
        //    {
        //        List<int> list = new List<int>();
        //        list.Add(2017);
        //        list.Add(2018);
        //        list.Add(2019);
        //        list.Add(2020);
        //        list.Add(2021);
        //        list.Add(2022);
        //        list.Add(2023);
        //        list.Add(2024);
        //        list.Add(2024);
        //        list.Add(2025);
        //        list.Add(2026);
        //        list.Add(2027);
        //        list.Add(2028);
        //        list.Add(2029);
        //        list.Add(2030);
        //        if (list != null && list.Count() > 0)
        //        {
        //            List<YearEntity> ylist = new List<YearEntity>();
        //            foreach (var item in list)
        //            {
        //                YearEntity yEntity = new YearEntity();
        //                yEntity.YearID = item;

        //                ylist.Add(yEntity);
        //            }
        //            if (ylist != null && ylist.Count() > 0)
        //            {
        //                InsertMasterDataEntity insertMasterEntity = new InsertMasterDataEntity();
        //                insertMasterEntity.MasterDataJSON = Newtonsoft.Json.JsonConvert.SerializeObject(ylist);
        //                if (insertMasterEntity.MasterDataJSON != null)
        //                {
        //                    insertMasterEntity.PartitionKey = "Year";
        //                    insertMasterEntity.RowKey = "Year";
        //                    TableOperation insert = TableOperation.Insert(insertMasterEntity);
        //                    masterDataTable.ExecuteAsync(insert);
        //                }
        //            }
        //        }
        //    }
        //}
    }
    public static class EPPlusExtensions
    {
        public static List<ExcelData> ConvertStateSheetToObjects<T>(this ExcelWorksheet worksheet) where T : new()
        {
            Func<CustomAttributeData, bool> columnOnly = y => y.AttributeType == typeof(Column);

            var columns = typeof(T).GetProperties().Where(x => x.CustomAttributes.Any(columnOnly))
            .Select(p => new
            {
                Property = p,
                Column = p.GetCustomAttributes<Column>().First().ColumnIndex
            }).ToList();

            var rows = worksheet.Cells.Select(cell => cell.Start.Row).Distinct().Skip(1).OrderBy(x => x);
            List<ExcelData> list = new List<ExcelData>();

            ExcelData excelData = new ExcelData();

            foreach (var item in rows)
            {
                excelData = new ExcelData();
                foreach (var item1 in columns)
                {
                    var val = worksheet.Cells[item, item1.Column];

                    if (item1.Column == 1 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.PartitionKey = val.GetValue<string>();
                    }
                    if (item1.Column == 2 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.RowKey = val.GetValue<string>();
                    }
                    if (item1.Column == 3 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.AppointmentDateTime = val.GetValue<string>();
                    }
                    if (item1.Column == 4 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.AppointmentNumber = val.GetValue<string>();
                    }
                    if (item1.Column == 5 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.AppointmentStatus = val.GetValue<string>();
                    }
                    if (item1.Column == 6 && item1.Property.PropertyType == typeof(Int32))
                    {
                        excelData.AppointmentStatusID = val.GetValue<Int32>();
                    }
                    if (item1.Column == 7 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.CreatedBy = val.GetValue<string>();
                    }
                    if (item1.Column == 8 && item1.Property.PropertyType == typeof(DateTime))
                    {
                        excelData.CreatedTS = val.GetValue<DateTime>();
                    }
                    if (item1.Column == 9 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.Date = val.GetValue<string>();
                    }
                    if (item1.Column == 10 && item1.Property.PropertyType == typeof(Int32))
                    {
                        excelData.EndTime = val.GetValue<Int32>();
                    }
                    if (item1.Column == 11 && item1.Property.PropertyType == typeof(Int32))
                    {
                        excelData.ImageStatusID = val.GetValue<Int32>();
                    }
                    if (item1.Column == 12 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.RequestDate = val.GetValue<string>();
                    }
                    if (item1.Column == 13 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.RequestDateTime = val.GetValue<string>();
                    }
                    if (item1.Column == 14 && item1.Property.PropertyType == typeof(Int32))
                    {
                        excelData.RequestTime = val.GetValue<Int32>();
                    }
                    if (item1.Column == 15 && item1.Property.PropertyType == typeof(Int32))
                    {
                        excelData.StartTime = val.GetValue<Int32>();
                    }
                    if (item1.Column == 16 && item1.Property.PropertyType == typeof(Int32))
                    {
                        excelData.StatusID = val.GetValue<Int32>();
                    }
                    if (item1.Column == 17 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.UpdatedBy = val.GetValue<string>();
                    }
                    if (item1.Column == 18 && item1.Property.PropertyType == typeof(DateTime))
                    {
                        excelData.UpdatedTS = val.GetValue<DateTime>();
                    }
                    if (item1.Column == 19 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.UserID = val.GetValue<string>();
                    }
                    if (item1.Column == 20 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.UserName = val.GetValue<string>();
                    }
                    if (item1.Column == 21 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.DesignNumber = val.GetValue<string>();
                    }
                    if (item1.Column == 22 && item1.Property.PropertyType == typeof(string))
                    {
                        excelData.CancellationReason = val.GetValue<string>();
                    }
                }
                if (excelData != null)
                {
                    list.Add(excelData);
                }
            }
            return list;
        }

        //public static List<ExcelDataCountry> ConvertCountrySheetToObjects<T>(this ExcelWorksheet worksheet) where T : new()
        //{
        //    Func<CustomAttributeData, bool> columnOnly = y => y.AttributeType == typeof(Column);

        //    var columns = typeof(T).GetProperties().Where(x => x.CustomAttributes.Any(columnOnly))
        //    .Select(p => new
        //    {
        //        Property = p,
        //        Column = p.GetCustomAttributes<Column>().First().ColumnIndex
        //    }).ToList();

        //    var rows = worksheet.Cells.Select(cell => cell.Start.Row).Distinct().Skip(1).OrderBy(x => x);
        //    List<ExcelDataCountry> list = new List<ExcelDataCountry>();

        //    ExcelDataCountry excelData = new ExcelDataCountry();

        //    foreach (var item in rows)
        //    {
        //        excelData = new ExcelDataCountry();
        //        foreach (var item1 in columns)
        //        {
        //            var val = worksheet.Cells[item, item1.Column];

        //            if (item1.Column == 1 && item1.Property.PropertyType == typeof(Int32))
        //            {
        //                excelData.CountryID = val.GetValue<Int32>();
        //            }
        //            if (item1.Column == 2 && item1.Property.PropertyType == typeof(string))
        //            {
        //                excelData.Name = val.GetValue<string>();
        //            }
        //            if (item1.Column == 3 && item1.Property.PropertyType == typeof(string))
        //            {
        //                excelData.PostalCodeRegax = val.GetValue<string>();
        //            }
        //        }
        //        if (excelData != null)
        //        {
        //            list.Add(excelData);
        //        }
        //    }
        //    return list;
        //}

        //public static List<ExcelTimeSlotData> ConvertTimeSlotSheetToObjects<T>(this ExcelWorksheet worksheet) where T : new()
        //{
        //    Func<CustomAttributeData, bool> columnOnly = y => y.AttributeType == typeof(Column);

        //    var columns = typeof(T).GetProperties().Where(x => x.CustomAttributes.Any(columnOnly))
        //    .Select(p => new
        //    {
        //        Property = p,
        //        Column = p.GetCustomAttributes<Column>().First().ColumnIndex
        //    }).ToList();

        //    var rows = worksheet.Cells.Select(cell => cell.Start.Row).Distinct().Skip(1).OrderBy(x => x);
        //    List<ExcelTimeSlotData> list = new List<ExcelTimeSlotData>();

        //    ExcelTimeSlotData excelData = new ExcelTimeSlotData();

        //    foreach (var item in rows)
        //    {
        //        excelData = new ExcelTimeSlotData();
        //        foreach (var item1 in columns)
        //        {
        //            var val = worksheet.Cells[item, item1.Column];

        //            if (item1.Column == 1 && item1.Property.PropertyType == typeof(Int32))
        //            {
        //                excelData.TimeSlotID = val.GetValue<Int32>();
        //            }
        //            if (item1.Column == 2 && item1.Property.PropertyType == typeof(string))
        //            {
        //                excelData.Name = val.GetValue<string>();
        //            }
        //            if (item1.Column == 3 && item1.Property.PropertyType == typeof(string))
        //            {
        //                excelData.StartTime = val.GetValue<string>();
        //            }
        //            if (item1.Column == 4 && item1.Property.PropertyType == typeof(string))
        //            {
        //                excelData.EndTime = val.GetValue<string>();
        //            }
        //        }
        //        if (excelData != null)
        //        {
        //            list.Add(excelData);
        //        }
        //    }
        //    return list;
        //}
    }

    public class ExcelDataCountry
    {
        [Column(1)]
        public int CountryID { get; set; }
        [Column(2)]
        public string Name { get; set; }
        [Column(3)]
        public string PostalCodeRegax { get; set; }
    }

    public class ExcelData
    {
        [Column(1)]
        public string PartitionKey { get; set; }
        [Column(2)]
        public string RowKey { get; set; }
        [Column(3)]
        public string AppointmentDateTime { get; set; }
        [Column(4)]
        public string AppointmentNumber { get; set; }
        [Column(5)]
        public string AppointmentStatus { get; set; }
        [Column(6)]
        public int AppointmentStatusID { get; set; }
        [Column(7)]
        public string CreatedBy { get; set; }
        [Column(8)]
        public DateTime CreatedTS { get; set; }
        [Column(9)]
        public string Date { get; set; }
        [Column(10)]
        public int EndTime { get; set; }
        [Column(11)]
        public int ImageStatusID { get; set; }
        [Column(12)]
        public string RequestDate { get; set; }
        [Column(13)]
        public string RequestDateTime { get; set; }
        [Column(14)]
        public int RequestTime { get; set; }

        [Column(15)]
        public int StartTime { get; set; }

        [Column(16)]
        public int StatusID { get; set; }


        [Column(17)]
        public string UpdatedBy { get; set; }
        [Column(18)]
        public DateTime UpdatedTS { get; set; }
        [Column(19)]
        public string UserID { get; set; }
        [Column(20)]
        public string UserName { get; set; }


        [Column(21)]
        public string DesignNumber { get; set; }

        [Column(22)]
        public string CancellationReason { get; set; }



    }

    public class ExcelTimeSlotData
    {
        [Column(1)]
        public int TimeSlotID { get; set; }
        [Column(2)]
        public string Name { get; set; }
        [Column(3)]
        public string StartTime { get; set; }
        [Column(4)]
        public string EndTime { get; set; }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class Column : Attribute
    {
        public int ColumnIndex { get; set; }
        public Column(int column)
        {
            ColumnIndex = column;
        }
    }

}
