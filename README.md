Reference: "Slay The Spire in UNITY" by The Code Otter


Needed Assets:

-2D

-Splines

-DOTween (HOTween v2)
https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676

-Serialize Reference Editor
https://assetstore.unity.com/packages/tools/utilities/serialize-reference-editor-297559

1. 핵심 시스템 구조
시스템 이름	역할 요약
CardSystem:	덱/패/버림 관리 + 카드 드로우/사용 로직
ActionSystem:	GameAction 수행 및 Performer 관리
CharmMoneySystem:	Charm, Money 상태 추적 + GameAction 처리
CardViewCreator:	Card → CardView 생성 및 뷰 갱신
EffectSystem:	Effect 관리 → PerformEffectGA와 연계

2. 데이터 및 구성 요소
구성 요소	특징
CardData.cs:	ScriptableObject 기반 카드 정의 (Mana, Money, Charm 등)
GameAction.cs:	추상 클래스, 리액션 리스트 포함
Effect 파생 클래스:	다양한 카드 효과 구현 (DrawCardEffect 등)

3. 뷰/UI 관련 구조
클래스	역할
CardView.cs:	카드 UI 표시 + 마우스 상호작용
CardViewCreator:	카드 → 뷰 생성 및 관리
HandView:	손패 관리 (AddCard 등)
