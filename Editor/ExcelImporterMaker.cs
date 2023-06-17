using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Object = UnityEngine.Object;

public class ExcelImporterMaker : EditorWindow
{
    private enum ValueType
    {
        BOOL,
        STRING,
        INT,
        FLOAT,
        DOUBLE,
    }

    private string filePath = string.Empty;
    private bool isSeparateSheet = false;
    private List<ExcelRowParameter> typeList = new List<ExcelRowParameter>();
    private List<ExcelSheetParameter> sheetList = new List<ExcelSheetParameter>();
    private string className = string.Empty;
    private string fileName = string.Empty;
    private Vector2 currScroll = Vector2.zero;
    
    private static string KeyPrefix = "excel-importer-maker";

    private class ExcelSheetParameter
    {
        public string sheetName;
        public bool isEnable;
    }
    
    private class ExcelRowParameter
    {
        public ValueType type;
        public string name;
        public bool isEnable;
        public bool isArray;
        public ExcelRowParameter nextArrayItem;
    }

    private void OnGUI()
    {
        GUILayout.Label("making importer", EditorStyles.boldLabel);
        className = EditorGUILayout.TextField("class name", className);
        isSeparateSheet = EditorGUILayout.Toggle("separate sheet", isSeparateSheet);
        
        EditorPrefs.SetBool(KeyPrefix + fileName + ".separateSheet", isSeparateSheet);

        if (GUILayout.Button("create"))
        {
            EditorPrefs.SetString(KeyPrefix + fileName + ".className", className);
            ExportEntity();
            ExportImporter();
            Debug.Log(filePath);
            AssetDatabase.ImportAsset(filePath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Close();
        }
        
        // selecting sheets
        EditorGUILayout.LabelField("sheet settings");
        EditorGUILayout.BeginVertical("box");
        foreach (ExcelSheetParameter sheet in sheetList)
        {
            GUILayout.BeginHorizontal();
            sheet.isEnable = EditorGUILayout.BeginToggleGroup("enable", sheet.isEnable);
            EditorGUILayout.LabelField(sheet.sheetName);
            EditorGUILayout.EndToggleGroup();
            EditorPrefs.SetBool(KeyPrefix + fileName + ".sheet." + sheet.sheetName, sheet.isEnable);
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        
        // selecting parameters
        EditorGUILayout.LabelField("parameter settings");
        currScroll = EditorGUILayout.BeginScrollView(currScroll);
        EditorGUILayout.BeginVertical("box");
        string lastCellName = string.Empty;
        foreach (ExcelRowParameter cell in typeList)
        {
            if (cell.isArray && lastCellName != null && cell.name.Equals(lastCellName))
                continue;

            cell.isEnable = EditorGUILayout.BeginToggleGroup("enable", cell.isEnable);
            if (cell.isArray)
                EditorGUILayout.LabelField("---[array]---");
            GUILayout.BeginHorizontal();
            cell.name = EditorGUILayout.TextField(cell.name);
            cell.type = (ValueType)EditorGUILayout.EnumPopup(cell.type, GUILayout.MaxWidth(100));
            EditorPrefs.SetInt(KeyPrefix + fileName + ".type." + cell.name, (int)cell.type);
            GUILayout.EndHorizontal();
            
            EditorGUILayout.EndToggleGroup();
            lastCellName = cell.name;
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    [MenuItem("Assets/XLS Import Settings...")]
    private static void ExportExcelToAssetBundle()
    {
        foreach (Object obj in Selection.objects)
        {
            ExcelImporterMaker window = CreateInstance<ExcelImporterMaker>();
            window.filePath = AssetDatabase.GetAssetPath(obj);
            window.fileName = Path.GetFileNameWithoutExtension(window.filePath);

            if (window.filePath == null)
                continue;
            
            using (FileStream stream = File.Open(window.filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook book = new HSSFWorkbook(stream);
                ISheet sheet = null;
                
                for (int i = 0; i < book.NumberOfSheets; i++)
                {
                    sheet = book.GetSheetAt(i);
                    ExcelSheetParameter sheetParam = new ExcelSheetParameter();
                    sheetParam.sheetName = sheet.SheetName;
                    sheetParam.isEnable = EditorPrefs.GetBool(KeyPrefix + window.fileName + ".sheet." + sheetParam.sheetName, true);
                    window.sheetList.Add(sheetParam);
                }

                sheet = book.GetSheetAt(0);
                window.className = EditorPrefs.GetString(KeyPrefix + window.fileName + ".className", "Entity_" + sheet.SheetName);
                window.isSeparateSheet = EditorPrefs.GetBool(KeyPrefix + window.fileName + ".separateSheet");

                IRow titleRow = sheet.GetRow(0);
                IRow dataRaw = sheet.GetRow(1);
                for (int i = 0; i < titleRow.LastCellNum; i++)
                {
                    ExcelRowParameter lastParser = null;
                    ExcelRowParameter parser = new ExcelRowParameter();
                    parser.name = titleRow.GetCell(i).StringCellValue;
                    parser.isArray = parser.name.Contains("[]");
                    if (parser.isArray)
                        parser.name = parser.name.Remove(parser.name.LastIndexOf("[]", StringComparison.Ordinal));

                    ICell cell = dataRaw.GetCell(i);
                    if (cell == null)
                        continue;
                    
                    // array support
                    if (window.typeList.Count > 0)
                    {
                        lastParser = window.typeList[window.typeList.Count - 1];
                        if (lastParser.isArray && parser.isArray && lastParser.name.Equals(parser.name))
                        {
                            parser.isEnable = lastParser.isEnable;
                            parser.type = lastParser.type;
                            lastParser.nextArrayItem = parser;
                            window.typeList.Add(parser);
                            continue;
                        }
                    }

                    if (cell.CellType != CellType.Unknown && cell.CellType != CellType.BLANK)
                    {
                        parser.isEnable = true;

                        try
                        {
                            if (EditorPrefs.HasKey(KeyPrefix + window.fileName + ".type." + parser.name))
                                parser.type = (ValueType)EditorPrefs.GetInt(KeyPrefix + window.fileName + ".type." + parser.name);
                            else
                                parser.type = ValueType.STRING;
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e.Message);
                        }
                    }
                    
                    window.typeList.Add(parser);
                }

                window.Show();
            }
        }
    }

    private void ExportEntity()
    {
        string templateFilePath = (isSeparateSheet) ? "Assets/Editor/EntityTemplateSeparate.txt" : "Assets/Editor/EntityTemplate.txt";
        string entityTemplate = File.ReadAllText(templateFilePath);
        StringBuilder builder = new StringBuilder();
        bool isInBetweenArray = false;
        foreach (ExcelRowParameter row in typeList)
        {
            if (row.isEnable)
            {
                if (!row.isArray)
                {
                    builder.AppendLine();
                    builder.AppendFormat("		public {0} {1};", row.type.ToString().ToLower(), row.name);
                }
                else
                {
                    if (!isInBetweenArray)
                    {
                        builder.AppendLine();
                        builder.AppendFormat("		public {0}[] {1};", row.type.ToString().ToLower(), row.name);
                    }

                    isInBetweenArray = (row.nextArrayItem != null);
                }
            }
        }

        entityTemplate = entityTemplate.Replace("$Types$", builder.ToString());
        entityTemplate = entityTemplate.Replace("$ExcelData$", className);

        builder.Clear();

        builder.AppendLine();
        builder.Append("		public override string ToString()");
        builder.AppendLine();
        builder.Append("		{");
        builder.AppendLine();
        builder.Append("			StringBuilder builder = new StringBuilder();");
        builder.AppendLine();
        foreach (ExcelRowParameter row in typeList)
        {
	        if (row.isEnable)
	        {
		        if (!row.isArray)
		        {
			        builder.AppendLine();
			        builder.AppendFormat("			builder.Append(\"{0} : \" + {1} + \" / \");", row.name, row.name);
			        builder.AppendLine();
		        }
		        else
		        {
			        if (!isInBetweenArray)
			        {
				        builder.AppendLine();
				        builder.AppendFormat("			builder.Append(\"{0} : [\");", row.name);
				        builder.AppendLine();
				        builder.AppendFormat("			foreach ({0} element in {1})", row.type.ToString().ToLower(), row.name);
				        builder.AppendLine();
				        builder.Append("				builder.AppendFormat(\"{0}, \", element);");
				        builder.AppendLine();
				        builder.Append("			builder.Append(\"] / \");");
				        builder.AppendLine();
			        }

			        isInBetweenArray = (row.nextArrayItem != null);
		        }
	        }
        }
        builder.AppendLine();
        builder.Append("			return builder.ToString();");
        builder.AppendLine();
        builder.Append("		}");

        entityTemplate = entityTemplate.Replace("$ToString$", builder.ToString());
        
        Directory.CreateDirectory("Assets/Classes/");
        string path = "Assets/Classes/" + className + ".cs";
        if (File.Exists(path))
            File.Delete(path);
        File.WriteAllText(path, entityTemplate);
    }

    private void ExportImporter()
    {
        string templateFilePath = (isSeparateSheet) ? "Assets/Editor/ExportTemplateSeparate.txt" : "Assets/Editor/ExportTemplate.txt";
        string importerTemplate = File.ReadAllText(templateFilePath);

        StringBuilder builder = new StringBuilder();
        StringBuilder sheetListBuilder = new StringBuilder();
        int rowCount = 0;
        string tab = "						";
        bool isInBetweenArray = false;
        
        // $SheetList$
        foreach (ExcelSheetParameter sheet in sheetList)
        {
            if (sheet.isEnable)
                sheetListBuilder.Append("\"" + sheet.sheetName + "\",");
        }

        foreach (ExcelRowParameter row in typeList)
        {
            if (row.isEnable)
            {
                if (!row.isArray)
                {
                    builder.AppendLine();
                    switch (row.type)
                    {
                        case ValueType.BOOL:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (cell == null ? false : cell.BooleanCellValue);", row.name, rowCount);
                            break;
                        case ValueType.INT:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (cell == null ? 0 : (int)cell.NumericCellValue);", row.name, rowCount);
                            break;
                        case ValueType.FLOAT:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (cell == null ? 0.0f : (float)cell.NumericCellValue);", row.name, rowCount);
                            break;
                        case ValueType.DOUBLE:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (cell == null ? 0.0 : cell.NumericCellValue);", row.name, rowCount);
                            break;
                        case ValueType.STRING:
                            builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0} = (cell == null ? \"\" : cell.StringCellValue);", row.name, rowCount);
                            break;
                    }
                }
                else
                {
                    if (!isInBetweenArray)
                    {
                        int arrayLength = 0;
                        for (ExcelRowParameter r = row; r != null; r = r.nextArrayItem, arrayLength++) {}

                        builder.AppendLine();
                        switch (row.type)
                        {
                            case ValueType.BOOL:
                                builder.AppendFormat(tab + "p.{0} = new bool[{1}];", row.name, arrayLength);
                                break;
                            case ValueType.INT:
                                builder.AppendFormat(tab + "p.{0} = new int[{1}];", row.name, arrayLength);
                                break;
                            case ValueType.FLOAT:
                                builder.AppendFormat(tab + "p.{0} = new float[{1}];", row.name, arrayLength);
                                break;
                            case ValueType.DOUBLE:
                                builder.AppendFormat(tab + "p.{0} = new double[{1}];", row.name, arrayLength);
                                break;
                            case ValueType.STRING:
                                builder.AppendFormat(tab + "p.{0} = new string[{1}];", row.name, arrayLength);
                                break;
                        }

                        for (int i = 0; i < arrayLength; i++)
                        {
                            builder.AppendLine();
                            switch (row.type)
                            {
                                case ValueType.BOOL:
                                    builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0}[{2}] = (cell == null ? false : cell.BooleanCellValue);", row.name, rowCount + i, i);
                                    break;
                                case ValueType.INT:
                                    builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0}[{2}] = (cell == null ? 0 : (int)cell.NumericCellValue);", row.name, rowCount + i, i);
                                    break;
                                case ValueType.FLOAT:
                                    builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0}[{2}] = (cell == null ? 0.0f : (float)cell.NumericCellValue);", row.name, rowCount + i, i);
                                    break;
                                case ValueType.DOUBLE:
                                    builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0}[{2}] = (cell == null ? 0.0 : cell.NumericCellValue);", row.name, rowCount + i, i);
                                    break;
                                case ValueType.STRING:
                                    builder.AppendFormat(tab + "cell = row.GetCell({1}); p.{0}[{2}] = (cell == null ? \"\" : cell.StringCellValue);", row.name, rowCount + i, i);
                                    break;
                            }
                        }
                    }
                    isInBetweenArray = (row.nextArrayItem != null);
                }
            }
            rowCount += 1;
        }

        importerTemplate = importerTemplate.Replace("$IMPORT_PATH$", this.filePath);
        importerTemplate = importerTemplate.Replace("$ExportAssetDirectory$", Path.GetDirectoryName(this.filePath)?.Replace("\\", "/"));
        importerTemplate = importerTemplate.Replace("$EXPORT_PATH$", Path.ChangeExtension(this.filePath, ".asset"));
        importerTemplate = importerTemplate.Replace("$ExcelData$", className);
        importerTemplate = importerTemplate.Replace("$SheetList$", sheetListBuilder.ToString());
        importerTemplate = importerTemplate.Replace("$EXPORT_DATA$", builder.ToString());
        importerTemplate = importerTemplate.Replace("$ExportTemplate$", fileName + "Importer");

        Directory.CreateDirectory("Assets/Classes/Editor/");

        string path = "Assets/Classes/Editor/" + fileName + "Importer.cs";
        if (File.Exists(path))
            File.Delete(path);
        File.WriteAllText(path, importerTemplate);
    }
}