using UnityEngine;
using System.IO;
using UnityEditor;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

public class TextTableImporter : AssetPostprocessor
{
    private static readonly string filePath = "Assets/Excels/TextTable.xls";
    private static readonly string[] sheetNames = { "TextTable", };
    
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets)
        {
            if (!filePath.Equals(asset))
                continue;

            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook book = new HSSFWorkbook(stream);

                foreach (string sheetName in sheetNames)
                {
                    string exportPath = "Assets/Excels/" + sheetName + ".asset";
                    
                    // check scriptable object
                    TextTable data = (TextTable)AssetDatabase.LoadAssetAtPath(exportPath, typeof(TextTable));
                    if (data == null)
                    {
                        data = ScriptableObject.CreateInstance<TextTable>();
                        AssetDatabase.CreateAsset(data, exportPath);
                        data.hideFlags = HideFlags.NotEditable;
                    }
                    data.paramList.Clear();

					// check sheet
                    ISheet sheet = book.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        Debug.LogError("[Data] sheet not found:" + sheetName);
                        continue;
                    }

                	// add information
                    for (int i = 1; i < sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        ICell cell = null;
                        
                        TextTable.Param p = new TextTable.Param();
			            
						cell = row.GetCell(0); p.id = (cell == null ? 0 : (int)cell.NumericCellValue);
						p.texts = new string[3];
						cell = row.GetCell(1); p.texts[0] = (cell == null ? "" : cell.StringCellValue);
						cell = row.GetCell(2); p.texts[1] = (cell == null ? "" : cell.StringCellValue);
						cell = row.GetCell(3); p.texts[2] = (cell == null ? "" : cell.StringCellValue);

                        data.paramList.Add(p);
                    }
                    
                    // save scriptable object
                    ScriptableObject obj = AssetDatabase.LoadAssetAtPath(exportPath, typeof(ScriptableObject)) as ScriptableObject;
                    EditorUtility.SetDirty(obj);
                }
            }
        }
    }
}