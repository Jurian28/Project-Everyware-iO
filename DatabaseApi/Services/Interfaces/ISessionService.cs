using SharedClassLibrary.DTOs.Sessions;

namespace DatabaseApi.Services.Interfaces;

public interface ISessionService
{
    public Task<List<SessionSpotsDTO>> ReturnSessionSpotsData(int eventId, int? sessionId);
}
