using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Unknown,
    Preset,
    Plugin,
    Service,
    AddressableSO,
    AddressableUI,
    AddressablePrefab,
    Dependency
}

public interface INodeData
{
    string Label { get; }             // 노드 타이틀 (Preset 이름 / Plugin 이름)
    Dictionary<NodeType, string[]> ContentsByType { get; } // 노드 구성 요소를 타입별로 정리한 데이터
    Vector2 Position { get; set; }    // 노드 위치
    Vector2 Size { get; set; }        // 노드 크기
    Color Color { get; }              // 타입 기반 색상
    UnityEngine.Object TargetObject { get; } // 선택 시 표시할 대상
}