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
        public static JobType SelectedJob => mainData.SelectJobData.SelectedJob;
        public static void SetSelectedJob(JobType jobType)
        {
            mainData.SelectJobData.SelectedJob = jobType;
            Save("Set SelectedJob");
        }

        public static JobType GetNextJob()
        {
            return (JobType)(((int)SelectedJob + 1) % Enum.GetValues(typeof(JobType)).Length);
        }
        public static JobType GetLastJob()
        {
            return (JobType)(((int)SelectedJob - 1 + Enum.GetValues(typeof(JobType)).Length) 
                % Enum.GetValues(typeof(JobType)).Length);
        }
        public static bool IsIronClad()
        {
            return SelectedJob == JobType.IronClad;
        }
    }
}

public class OnSelectedJobChangeEvent
{
    public JobType JobType;
}
