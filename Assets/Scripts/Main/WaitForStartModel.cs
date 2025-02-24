using System;

public enum JobType
{
    IronClad,
    Silent,
    JiBao,
    Watcher,
}
public partial class MainData
{
    public WaitForStartData WaitForStartData;
}

[Serializable]
public class WaitForStartData
{
    public JobType selectedJob;
    public WaitForStartData()
    {
        selectedJob = JobType.IronClad;
    }
}

public partial class MainModel
{
    public class WaitForStartModel
    {
        static WaitForStartData waitForStartData = mainData.WaitForStartData;

        public static void SetSelectedJob(JobType jobType)
        {
            waitForStartData.selectedJob = jobType;
            MainModel.Save();
        }
    }
}
