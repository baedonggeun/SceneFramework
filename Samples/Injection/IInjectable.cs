using System;

public interface IInjectable
{
    // 이 객체가 의존하는 대상들의 타입 목록을 반환
    Type[] GetDependencies();
}