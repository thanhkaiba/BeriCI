using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Sailor_importer : AssetPostprocessor
{
    private static readonly string filePath = "Assets/Terasurware/Sailor.xlsx";
    private static readonly string exportPath = "Assets/Terasurware/Sailor.asset";
    private static readonly string[] sheetNames = { "description", };

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets)
        {
            if (!filePath.Equals(asset))
                continue;

            SailorDescription data = (SailorDescription)AssetDatabase.LoadAssetAtPath(exportPath, typeof(SailorDescription));
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<SailorDescription>();
                AssetDatabase.CreateAsset((ScriptableObject)data, exportPath);
                data.hideFlags = HideFlags.NotEditable;
            }

            data.sheets.Clear();
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook book = null;
                if (Path.GetExtension(filePath) == ".xls")
                {
                    book = new HSSFWorkbook(stream);
                }
                else
                {
                    book = new XSSFWorkbook(stream);
                }

                foreach (string sheetName in sheetNames)
                {
                    ISheet sheet = book.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        Debug.LogError("[QuestData] sheet not found:" + sheetName);
                        continue;
                    }

                    SailorDescription.Sheet s = new SailorDescription.Sheet();
                    s.name = sheetName;

                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        ICell cell = null;

                        SailorDescription.Param p = new SailorDescription.Param();

                        cell = row.GetCell(1); p.root_name = (cell == null ? "" : cell.StringCellValue);
                        cell = row.GetCell(2); p.present_name = (cell == null ? "" : cell.StringCellValue);
                        cell = row.GetCell(3); p.title = (cell == null ? "" : cell.StringCellValue);
                        cell = row.GetCell(4); p.skill_description = (cell == null ? "" : cell.StringCellValue);
                        s.list.Add(p);
                    }
                    data.sheets.Add(s);
                }
            }

            ScriptableObject obj = AssetDatabase.LoadAssetAtPath(exportPath, typeof(ScriptableObject)) as ScriptableObject;
            EditorUtility.SetDirty(obj);
        }
    }
}
