using System;
using UnityEngine;

public enum JobType
{
    IronClad,
    Silent,
    JiBao,
    Watcher,
}
public partial class MainData
{
    public SelectJobData SelectJobData;
}

[Serializable]
public class SelectJobData
{
    [SerializeField]
    JobType selectedJob;
    public JobType SelectedJob
    {
        get
        {
            return selectedJob;
        }
        set
        {
            selectedJob = value;
            MyEvent.Fire(new OnSelectedJobChangeEvent()
            {
                JobType = value
            });
        }
    }
}

public partial class MainModel 
{
    public class SelectJobModel
    {
        public void EnterSelectJob()
        {
            if (mainData.SelectJobData == null)
            {
                mainData.SelectJobData = new()
                {
                    SelectedJob = JobType.IronClad,
                };
                Save("Init SelectJobData");
            }
        }
        public static JobType GetSelectedJob()
        {
            return mainData.SelectJobData.SelectedJob;
        }   
        public static void SetSelectedJob(JobType jobType)
        {
            mainData.SelectJobData.SelectedJob = jobType;
            Save("Set SelectedJob");
        }

        public static JobType GetNextJob()
        {
            return (JobType)(((int)mainData.SelectJobData.SelectedJob + 1) % Enum.GetValues(typeof(JobType)).Length);
        }
        public static JobType GetLastJob()
        {
            return (JobType)(((int)mainData.SelectJobData.SelectedJob - 1 + Enum.GetValues(typeof(JobType)).Length) 
                % Enum.GetValues(typeof(JobType)).Length);
        }
        public static bool IsIronClad()
        {
            return mainData.SelectJobData.SelectedJob == JobType.IronClad;
        }
    }
}

public class OnSelectedJobChangeEvent
{
    public JobType JobType;
}
