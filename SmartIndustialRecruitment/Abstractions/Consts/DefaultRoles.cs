namespace SmartIndustrialRecruitment.Abstractions.Consts;

public static class DefaultRoles
{
    public const string Admin = nameof(Admin);
    public const string AdminRoleId = "01988dc9-8541-7470-847d-eb058524f475";
    public const string AdminRoleConcurrencyStamp = "01988dc9-8542-7feb-bc91-64a85a443cce";
    
    public const string Worker = nameof(Worker);
    public const string WorkerRoleId = "019d9316-9d9f-7ca5-8522-688c4c04c0c5";
    public const string WorkerConcurrencyStamp = "019d9316-9d9f-7325-bf08-a7d3f5cc3f05";

    public const string Employer = nameof(Employer);
    public const string EmployerRoleId = "019d9316-9d9f-79c3-9bee-040b17cdeae0";
    public const string EmployerConcurrencyStamp = "019d9316-9d9f-72b3-aa84-38a74896df29";
}