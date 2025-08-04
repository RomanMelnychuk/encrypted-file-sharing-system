namespace FileGuard.Storage.Interfaces;

public interface IFolderService
{
    string Create(string folderPath);
    void Delete(string folderPath);

    void DeleteAndEmptyUserFolder(string folderPath);
}
