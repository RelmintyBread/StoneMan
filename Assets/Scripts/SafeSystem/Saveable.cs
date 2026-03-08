public interface ISaveable
{
    void OnSave(SaveData data);
    void OnLoad(SaveData data);
}