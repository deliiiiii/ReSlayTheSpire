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
    public SelectJobData SelectJobData = new();
}

[Serializable]
public class SelectJobData : IData<SelectJobData>
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
    public SelectJobData()
    {
        SelectedJob = JobType.IronClad;
    }   
    public void ReadData(SelectJobData savedData)
    {
        SelectedJob = savedData.SelectedJob;
    }

}

public partial class MainModel 
{
    public class SelectJobModel
    {
        static SelectJobData selectJobData = mainData.SelectJobData;


        public static void SetSelectedJob(JobType jobType)
        {
            selectJobData.SelectedJob = jobType;
            Save();
        }

        public static JobType GetNextJob()
        {
            return (JobType)(((int)selectJobData.SelectedJob + 1) % Enum.GetValues(typeof(JobType)).Length);
        }
        public static JobType GetLastJob()
        {
            return (JobType)(((int)selectJobData.SelectedJob - 1 + Enum.GetValues(typeof(JobType)).Length) % Enum.GetValues(typeof(JobType)).Length);
        }
        public static bool IsIronClad()
        {
            return selectJobData.SelectedJob == JobType.IronClad;
        }
    }
}

public class OnSelectedJobChangeEvent
{
    public JobType JobType;
}
