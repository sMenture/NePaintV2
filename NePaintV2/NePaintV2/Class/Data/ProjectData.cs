
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;


[Serializable]
public class ProjectData
{
    public string ProjectName { get; set; }
    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }
}


public class SaveProject
{
    private string GetAppDataFolder()
    {
        string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NePaint");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        return folderPath;
    }

    public void SaveProjectData(string nameProject, int width, int height)
    {
        var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
        saveFileDialog.Filter = "NePaint Project Files (*.npproj)|*.npproj";
        saveFileDialog.DefaultExt = "npproj";

        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            ProjectData projectData = new ProjectData
            {
                ProjectName = nameProject,
                CanvasWidth = width,
                CanvasHeight = height,
            };

            Save(saveFileDialog.FileName, projectData);
            UpdateProjectList(saveFileDialog.FileName);
        }
    }



    private void Save(string filePath, ProjectData projectData)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            formatter.Serialize(stream, projectData);
        }
    }

    private void UpdateProjectList(string filePath)
    {
        string appDataFolder = GetAppDataFolder();
        string projectListPath = Path.Combine(appDataFolder, "ProjectList.json");

        List<ProjectInfo> projectInfos = new List<ProjectInfo>();
        if (File.Exists(projectListPath))
        {
            string json = File.ReadAllText(projectListPath);
            projectInfos = JsonConvert.DeserializeObject<List<ProjectInfo>>(json);
        }

        ProjectInfo newProjectInfo = new ProjectInfo
        {
            FileName = Path.GetFileName(filePath),
            FilePath = filePath,
            SaveDate = DateTime.Now
        };

        projectInfos.Add(newProjectInfo);

        string updatedJson = JsonConvert.SerializeObject(projectInfos, Formatting.Indented);
        File.WriteAllText(projectListPath, updatedJson);
    }

    public ProjectData LoadProject(string filePath)
    {
        IFormatter formatter = new BinaryFormatter();
        using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            return (ProjectData)formatter.Deserialize(stream);
        }
    }
}


public class ProjectInfo
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public DateTime SaveDate { get; set; }
}



