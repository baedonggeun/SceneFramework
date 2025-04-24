using System.Collections.Generic;

public interface IPresetValidationRule
{
    void Validate(ScenePresetSO preset, List<ScenePluginSO> plugins);
}