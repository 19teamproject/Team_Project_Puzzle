# Unity 3D 퍼즐게임 
  내일배움캠프 숙련과제 팀프로젝트
  
## 프리즘 큐브
> Unity 3D로 개발한 퍼즐 형식의 실습 팀 프로젝트<br>
> 캐릭터 및 장착 아이템, 카메라 3종의 다양한 구도, 스테이지 4종의 컨텐츠가 구현되어있습니다.<br>
> 내일배움캠프 Unity 숙련주차 팀 프로젝트 2025.03.12 ~ 2025.03. 19


## 기술 스택
- Unity 2022.3.17f1
- C#
- Blender
- DoTween
- Naughty Attribute
- PhotoShop
- Json

## 개발
| 이름      | 파트               |
|:---------:|:------------------:|
| 김진홍(팀장) | 기계장치 스테이지 |
| 강진규     | 거울반사 스테이지 |
| 윤동영     | 큐브 스테이지 |
| 김학영     | 원소 스테이지|
| 박지원    | 전체 총괄, 코어 기능, 디자인, 타이틀 씬 UI |
</details>

## 게임 영상
[![동영상 설명](https://velog.velcdn.com/images/ehddud9608/post/19735ee3-73a9-49bd-bfbf-c9e7aebcdf75/image.png)](https://youtu.be/pdtYMgZHiv4)

## 조작
| 키 입력 | 동작    |
|:-----------:|:-------:|
| `W,S,A,D`       |  상,하,좌,우 이동   |
| `SPACE` |  점프 |
| `SHIFT` |  달리기 |
| `E`       | 오브젝트 상호작용  |
| `F` |  아이템 장착 |
| `마우스 휠` |  아이템 슬롯 이동 |
| `V` |  카메라 시점 변경 |
| `~` |  인게임 UI |


## 주요 기능
### 🏛️로비 화면
<img src="https://velog.velcdn.com/images/ehddud9608/post/1af29ce4-2cbc-46ed-a2d2-288dd6a0c35d/image.png" width="500">

로비 화면을 통해서 게임 스테이지를 선택하거나 클리어하지 못한 스테이즈를 도전할 수 있습니다.

### 🪜스테이지 설명
<img src="https://velog.velcdn.com/images/ehddud9608/post/efdb5ac4-23a7-46a3-922b-14d24c1cf835/image.png" width="500">

색깔별로 다른 점프, 텔레포트, 스케일 증가,감소 등을 이용해 목표지점까지 도달해야 합니다.

<img src="https://velog.velcdn.com/images/ehddud9608/post/00ed6a0a-ac60-4a95-a8a9-bd86b638bc30/image.png" width="500">

불, 물, 나무 등 원소를 사용하여 기믹을 해결하고 목표지점까지 도달해야 합니다.

<img src="https://velog.velcdn.com/images/ehddud9608/post/152604bf-9877-4788-b61a-c55d435a68ab/image.png" width="500">

거울의 빛 반사와 반사된 빛의 색상 혼합을 이용하여 목표 오브젝트의 색깔과 같은 빛이 접촉되어 <br>기믹을 해결해야 합니다.

<img src="https://velog.velcdn.com/images/ehddud9608/post/51fcc749-dec8-427e-b3ce-04430d3a1f0b/image.png" width="500">

각종 기계장치(케이블, 발판, 레버)등을 이용하여 기믹을 해결하고, 목표지점까지 도달해야 합니다.


### 🎥 카메라 구현

<img src="https://velog.velcdn.com/images/ehddud9608/post/52b32648-66d7-4521-947b-872dd62d8274/image.png" width="300">
<img src="https://velog.velcdn.com/images/ehddud9608/post/720abf9d-bc34-4bd7-aab7-a6d922cab3a8/image.png" width="300">

시네머신을 적용하여 카메라의 시점을 1인칭, 3인칭으로 자연스럽게 전환이 가능합니다.

### 🎨포스트 프로세싱

<img src="https://velog.velcdn.com/images/ehddud9608/post/82d398e0-b4c0-4bf4-9f2c-1ae7ac9a06e0/image.png" width="300">
<img src="https://velog.velcdn.com/images/ehddud9608/post/bb148c8e-dc73-4b2f-970c-d135dcdf4cf9/image.png" width="300">

포스트 프로세싱 V2 패키지를 이용하여 보다 더 역동적인 그래픽을 구현하였습니다.

## 트러블 슈팅
### 김진홍님
<img src="https://velog.velcdn.com/images/ehddud9608/post/013085c3-3f43-4353-884d-85166d7d1dae/image.png" width="300">
<img src="https://velog.velcdn.com/images/ehddud9608/post/87fead48-708b-4f60-8cc3-613cf0ef0982/image.png"width="300">

```markdown
문제 발생
    - 힌지 조인트를 활용하여 케이블을 만들어보았으나 콜라이더끼리 서로 충돌이 일어나서 원하는 결과를 얻지 못했습니다.

원인  
    - 힌지 조인트는 두 리지드바디를 묶어서 힌지에 연결된 것과 같이 움직이도록 제약을 두기 때문에, 자유로운 이동이 필요한 케이블을 만드는 데에는 적합하지 않았습니다.

해결책 
    - 스프링 조인트로 대체하고, 가운데 연결하고 있는 리지드바디들의 무게를 줄여 해결했습니다.

프로젝트 하면서 느낀점 
    - 음 구현해보는 기능이다보니 많은 시행착오가 있었지만, 그 과정에서 많은 것을 배웠습니다.다음에는 힌지 조인트를 활용하여 문이나 사슬을 구현해보고 싶다는 생각이 들었습니다.
```
### 강진규님
<img src="https://teamsparta.notion.site/image/attachment%3A2a9451b3-3824-4d8b-a1d1-a69d3d0628d2%3Ac3f63758-e317-444b-9704-397906ac14e8.png?table=block&id=1ba2dc3e-f514-8018-a851-ca1f29fa8bd2&spaceId=83c75a39-3aba-4ba4-a792-7aefe4b07895&width=580&userId=&cache=v2" width="300">
<img src="https://teamsparta.notion.site/image/attachment%3A74a3e246-8fc4-4007-94a8-05058d9180ef%3Aimage.png?table=block&id=1ba2dc3e-f514-8021-91d2-e71ff564a6f2&spaceId=83c75a39-3aba-4ba4-a792-7aefe4b07895&width=660&userId=&cache=v2" width="300"> 

```markdown
문제 발생  
    - 빛 생성기에서 생성된 빛이 거울에 반사되게 만들었는데 특정 상황에서 거울에 반사가 되면 유니티 프로그램 자체가 멈췄다.

원인  
    - 생성된 빛기둥의 레이캐스트가 자기 자신을 컨택하게 되었고 코드상으로 contiune를 사용하는 부분때문에 무한 루프에 빠져버린것이었다.

해결책 
    - 해당 부분을 break를 통해 자기 자신을 컨택하면 바로 while문을 탈출 할 수 있게 수정해 주었고 해당 문제는 바로 해결되었다.

프로젝트를 하면서 느낀 점 
    - 빛 반사라는 유니크한 기믹을 현 해볼 수 있는 기회라서 너무 좋았고 그만큼 어려웠다.반사 되는 부분의 최적화가 더 필요하다는 생각이 들었다. 다음 번에 만들게 되면 그 부분을 더 신경 써봐야겠다.
```

### 윤동영님
<img src="https://teamsparta.notion.site/image/attachment%3Abf196875-825d-4651-a9d5-abdc0e0f0c56%3Aimage.png?table=block&id=1ba2dc3e-f514-8048-a0be-f8c4fd06152b&spaceId=83c75a39-3aba-4ba4-a792-7aefe4b07895&width=660&userId=&cache=v2" width="300"> 
<img src="https://teamsparta.notion.site/image/attachment%3A2e3290d8-2258-476c-b5ba-1e06933fe8c1%3Aimage.png?table=block&id=1ba2dc3e-f514-8047-8a6e-e4e97be47ca0&spaceId=83c75a39-3aba-4ba4-a792-7aefe4b07895&width=660&userId=&cache=v2" width="300">

```markdown
문제 발생 
    - Damage Indicator의 Post-process Volume값을 장애물(Obstacle Cube)을 벗어났을 때 점진적으로 변화하도록 만들고 싶은데  Damage Indicator, Obstacle Cube의 연결과 애니메이션을 통한 다른 오브젝트에서의 상호작용에서의 어려움. 

원인  
    - Obstacle Cube에서 Trigger가 되었을 때 발생할 연출이기 때문에 Animation을 Cube에서 해결하려했고, Component로 추가되지 않은 값을 바꾸려하니 당연히 Animation을 만들 수 없었음.  

해결책 

    - 점진적으로 만들기 위한 Animation은 Damage Indicator에 만들어 붙이고, Obstacle Cube에서 `OnTriggerEnter` 메서드에 Damage Indicator의 애니메이션을 호출하는 방식
    - DOtween을 활용하여 Obstacle Cube 스크립트에서 바로 `OnTriggerEnter` 메서드에 값을 애니메이션처럼 변경해주는 방식

프로젝트 하면서 느낀점 
    - 애니메이션에 매번 Transform을 바꾸고, 이미지의 Sprite만 바꾸다보니 애니메이션은 플레이어의 위치를 바꾸거나 이미지를 바꾸거나라고만 편협한 내 편견이었다.
    - 튜터님께 들어보니 애니메이션을 통해서 Unity 모든 컴포넌트의 값을 바꿀 수 있고, 애니메이션 및 DOtween을 자연스럽게 쓰도록 연습을 많이 해야겠다.
```

### 김학영님
<img src="https://teamsparta.notion.site/image/attachment%3A28406254-8915-44ce-a268-e08fff0f7521%3Aimage.png?table=block&id=1ba2dc3e-f514-80bd-8863-f1c0b91d7d19&spaceId=83c75a39-3aba-4ba4-a792-7aefe4b07895&width=820&userId=&cache=v2" width="300">

```markdown
문제 발생
    - 인벤토리에 들어간 횃불을 꺼내는 키를 눌러도(즉, F키를 눌러도) 횃불이 꺼내지지 않는 오류가 발생했다. 
    - 디버깅을 수차례 실시한 결과, UiInventory에 있는 OnUse 함수(F키를 입력하면 아이템을 손에 들리게 해주는 함수)가 제대로 작동되지 않는 것을 발견하였다.

원인
    - Send Message의 경우, Player Input이 부착된 게임 오브젝트의 모든 컴포넌트에서 일치하는 함수를 찾아 호출하는 방식이다. 
    - 하지만 그 오브젝트를 넘어 다른 오브젝트에 있는 함수를 호출하지는 않는다.
    - 내가 호출하려고 했던 OnUse 함수(인벤토리 안에 있는 도구를 꺼내어 사용하게 해주는 함수)는 UI라는 다른 오브젝트의 하위에 붙어있었다. 그러므로, 호출이 안 되는 것이다.

해결책
    - Send Message 방식 대신 Invoke Unity Events 방식을 사용하면 된다.

프로젝트 하면서 느낀 점 
    - 고수 분들의 코드를 보면서 공부가 많이 되었다. 
    - DOtween 추천을 많이 받았는데 다음 프로젝트 때는 한층 더 레벨업하여 적용해봐야겠다. 재미있었습니다!
```

### 박지원님

<img src="https://teamsparta.notion.site/image/attachment%3A2d26ef7c-f0a1-4b1e-86ee-1099541ad5dd%3Aimage.png?table=block&id=1ba2dc3e-f514-80dd-8ecb-c4811ce04025&spaceId=83c75a39-3aba-4ba4-a792-7aefe4b07895&width=1060&userId=&cache=v2" width="300">

```markdown
문제 발생
    - 유니티에서 모서리가 깎인 큐브를 transform.localScale을 사용하여 조정하면 형태가 망가지는 문제 발생
    - 크기를 늘리면 모서리 부분까지 비율이 함께 변경되어 부드러운 엣지가 찌그러짐
    - 기존의 transform.localScale 방식은 큐브 전체를 균일하게 늘려버려 원하는 비율 조정이 불가능

원인
    - localScale을 직접 변경하면 모서리의 비율도 강제로 조정
    - 각 모서리와 엣지를 개별적으로 컨트롤할 수 있는 방법이 필요함
    - BoxCollider 크기와 중심점도 동적으로 업데이트되지 않아 충돌 감지에도 문제가 생김

해결책
    - 블렌더에서 큐브에 뼈대(Armature)를 추가하고, 유니티에서 개별적으로 제어하는 방식으로 변경
    - CubeBoneScaler.cs : DOTween을 활용해 뼈대를 이동하여 자연스러운 스케일 조정과 동시에 박스 콜라이더의 크기와 위치까지 계산하여 정상적으로 늘어날 수 있게 코드 작성

프로젝트 하면서 느낀점
    - 유니티에서 3D 모델을 다룰 때, localScale을 직접 변경하는 방식에는 한계가 많다고 느꼈습니다.
    - 문제를 해결하기 위해 여러 방법을 조사했고, 이번 프로젝트에서는 익숙한 툴(DOTween, Armature)을 활용해 빠르게 해결하는 방식을 선택했습니다.
    - 하지만, 모든 큐브에 뼈대를 적용하다 보니 불필요한 본(Bone)이 많아져 성능 부담이 발생했습니다.
    - 프로젝트가 끝난 후, 성능 최적화를 고려한 다른 방법들(쉐이더, 커스텀 메시 조작 등)도 연구하여 상황에 맞는 최적의 방식을 적용할 수 있도록 해야겠다고 생각했습니다.
```

## 라이선스
| 에셋 이름     |출처| 라이선스        |
|:-----------:|:---:|:-------------:|
| DoTween   |https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676| MIT License |
| Cable-physics   |  https://github.com/Hrober0/Cable-physics?tab=MIT-1-ov-file| MIT License |
| Little Dungeon Lever |    https://www.artstation.com/marketplace/p/5V2v1/little-dungeon-lever| Standard License |
| Lowpoly Cowboy RIO V1.1 |https://assetstore.unity.com/packages/3d/characters/humanoids/lowpoly-cowboy-rio-v1-1-288965| CC0 |
| Hierarchy Designer |    https://assetstore.unity.com/packages/tools/utilities/hierarchy-designer-273928| CC0 |
| Mystics of the Cosmos Music Copyright Free |https://www.youtube.com/watch?v=FVh6Dflv8IA| CC0 |
| Rustic Wooden Crates with Sturdy Design and Texture|https://stock.adobe.com/kr/search?k=wooden+crate+texture&asset_id=1087332404| Standard License |