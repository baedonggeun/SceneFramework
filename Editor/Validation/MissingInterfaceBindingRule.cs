using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
//서비스가 구체 클래스 타입으로만 InjectRegistry에 등록되고,인터페이스 타입으로는 등록되지 않았는지 검사
public class MissingInterfaceBindingRule : IPresetValidationRule
{
    public void Validate(ScenePresetSO preset, List<ScenePluginSO> plugins)
    {
        foreach (var servicePlugin in plugins.OfType<ServicePluginSO>())
        {
            foreach (var info in servicePlugin.RequiredServices)
            {
                Type concreteType = Type.GetType(info.typeName);
                if (concreteType == null) continue;

                object instance = CreateInstanceTemporarily(concreteType);
                if (instance == null) continue;

                var interfaces = concreteType.GetInterfaces()
                    .Where(i => i != typeof(IInjectable) && i != typeof(IInitializable));

                foreach (var iface in interfaces)
                {
                    bool isRegistered = plugins.Any(p =>
                        p.GetType() == concreteType || iface.IsAssignableFrom(p.GetType()));

                    if (!isRegistered)
                    {
                        Debug.LogWarning($"[PresetValidation] {preset.name} ▶ {concreteType.Name} → 인터페이스 {iface.Name} 미등록 (주입 누락 가능성)");
                    }
                }
            }
        }
    }

    // 실제로 등록하지 않고 타입만 평가하기 위한 더미 인스턴스 생성
    private object CreateInstanceTemporarily(Type type)
    {
        try
        {
            return Activator.CreateInstance(type, true);
        }
        catch
        {
            return null;
        }
    }
}
#endif