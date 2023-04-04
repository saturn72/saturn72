namespace MaintenanceHandler.Services
{
    public interface ICameraService
    {
        Task TakeSnapshot(string? area);
    }
}
