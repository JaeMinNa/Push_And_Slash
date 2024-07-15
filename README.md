# 🖥️ Push And Slash

+ 모든 적들을 떨어뜨리면 승리합니다!
+ 캐릭터를 수집하고 성장하세요!
+ 멀티 대전을 통해 1:1 PVP를 즐기세요!
+ 랭킹을 올려보세요!
<br/>

## 📽️ 개인 프로젝트 소개
 - 게임 이름 : Push And Slash
 - 플랫폼 : Android
 - 장르 : 3D 액션 멀티 PVP
 - 개발 기간 : 24.05.01 ~ 24.07.11
<br/>

## ⚙️ Environment

- `Unity 2022.3.23f1`
- **IDE** : Visual Studio 2019, MonoDevelop
- **VCS** : Git (GitHub Desktop)
- **Envrionment** : Android
- **Resolution** : 1920 x 1080 `FHD`
<br/>

## ▶️ 게임 스크린샷

<p align="center">
  <img src="https://github.com/user-attachments/assets/cf305438-8d78-436d-8180-afc30f376845" width="49%"/>
  <img src="https://github.com/user-attachments/assets/eb6542da-a840-4594-988c-dd1792e684da" width="49%"/>
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/d3eeb740-4e88-46ba-a48f-c600d23cc59f" width="49%"/>
  <img src="https://github.com/user-attachments/assets/8d75448f-daf8-45fc-9500-5963af37a0d3" width="49%"/>
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/a1cc2044-bc13-458e-adb6-9c3f677f8416" width="49%"/>
  <img src="https://github.com/user-attachments/assets/5aa4bb15-8753-419a-9858-bb4f1f961f55" width="49%"/>
</p>
<br/>

## 🔳 와이어 프레임
![image](https://github.com/user-attachments/assets/0099b12e-d1b4-4eeb-bbde-5fdb65517eed)


## 🧩 클라이언트 구조

### GameManager
![image](https://github.com/user-attachments/assets/38eb976c-8e82-4986-9e37-44602d08803a)

### Enemy
![image](https://github.com/user-attachments/assets/5a388a6d-9ddf-48c5-be22-65edad0331ef)


## ✏️ 구현 기능

### 1. 멀티 대전 입장
<img src="https://github.com/user-attachments/assets/ca915275-4091-425c-84de-1c4774e1dbed" width="50%"/>

#### 구현 이유
- PUN2 멀티 서버 연결
- PVP 시작 전, 대기방 구현

#### 구현 방법
- NetworkManager 생성 : 서버 접속, Room 생성 및 참가 관리
```C#
public void Connect()
{
    PhotonNetwork.ConnectUsingSettings();
}

public void JoinRandomOrCreateRoom()
{
    PhotonNetwork.JoinRandomOrCreateRoom(expectedMaxPlayers : 2, roomOptions : new RoomOptions() { MaxPlayers = 2 });
}

public override void OnJoinedRoom()
{
    Debug.Log("방참가완료");
    PhotonNetwork.Instantiate("PUN2/Room/RoomController", transform.position, Quaternion.identity);
}
``` 
​<br/>

- RoomController 생성 : OnPhotonSerializeView 함수를 통해, Room 데이터를 송수신
<img src="https://github.com/user-attachments/assets/4188147c-cc8d-45b1-a50b-b33c786f97c0" width="50%"/>
<br/>
<br/>

```C#
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        stream.SendNext(GameManager.I.DataManager.PlayerData.KoreaTag);
        stream.SendNext(GameManager.I.DataManager.PlayerData.Level);
        stream.SendNext(GameManager.I.DataManager.PlayerData.CharacterRank.ToString());
        stream.SendNext(GameManager.I.DataManager.GameData.UserName);
        stream.SendNext(GameManager.I.DataManager.GameData.RankPoint);
        stream.SendNext(GameManager.I.DataManager.PlayerData.Star);
        stream.SendNext(GameManager.I.DataManager.PlayerData.Tag);
    }
    else
    {
        _roomEnemyCharacterName = (string)stream.ReceiveNext();
        _roomEnemyCharacterLevel = (int)stream.ReceiveNext();
        _roomEnemyCharacterRank = (string)stream.ReceiveNext();
        _roomEnemyUserName = (string)stream.ReceiveNext();
        _roomEnemyRankPoint = (int)stream.ReceiveNext();
        _roomEnemyCharacterStar = (int)stream.ReceiveNext();
        _roomEnemyCharacterKorTag = (string)stream.ReceiveNext();
    }
}
```
<br/>

### 2. PUN2 멀티 채팅 구현
<img src="https://github.com/user-attachments/assets/3c3123b6-2357-4c31-8c5c-70267dd60e79" width="50%"/>

#### 구현 이유
- 입력한 string 데이터를 송수신

#### 구현 방법
- RPC 함수를 통해, 모든 Player가 동시에 함수 실행
```C#
private void SendChat()
{
    if (_photonView.IsMine)
    {
        string chat = PhotonNetwork.NickName + " : " + _networkManager.ChatInputText.text;
        _photonView.RPC("ChatRPC", RpcTarget.All, chat);
        _networkManager.ChatInputText.text = "";
    }
}

[PunRPC]
public void ChatRPC(string str)
{
    bool isInput = false;

    for (int i = 0; i < _chatTexts.Length; i++)
    {
        if (_chatTexts[i].text == "")
        {
            isInput = true;
            _chatTexts[i].text = str;
            break;
        }
    }

    if (!isInput)
    {
        for (int i = 1; i < _chatTexts.Length; i++)
        {
            _chatTexts[i - 1].text = _chatTexts[i].text;
        }

        _chatTexts[_chatTexts.Length - 1].text = str;
    }
}
```
<br/>

### 3. PUN2 멀티 애니메이션 동기화
<img src="https://github.com/user-attachments/assets/6baf68b3-0a0b-416c-924b-703abcb2b105" width="50%"/>

#### 구현 이유
- 멀티 PVP에서 애니메이션을 동기화

#### 구현 방법
- PhotonAnimatorView 컴포넌트 추가
<img src="https://github.com/user-attachments/assets/05fb9546-1e0b-41c4-8435-0e27bb8e57a3" width="50%"/>
<br/>
<br/>

- PhotonView 컴포넌트 추가 및 Observed Components에 PhotonAnimatorView 추가
<img src="https://github.com/user-attachments/assets/caa1a31b-b577-4593-b510-28426f1ff30c" width="50%"/>
<br/>
<br/>

- Synchronize Parameters에서 Bool Parameter를 Continuous 설정
- Trigger Parameter의 경우, RPC 함수를 통해 동기화해야 하므로, Disabled로 설정
<img src="https://github.com/user-attachments/assets/fe0ac5fd-96f7-44a6-8ec1-49d043f0c73a" width="50%"/>
<br/>
<br/>

### 4. PUN2 멀티 전투 구현
<img src="https://github.com/user-attachments/assets/eea1b5b6-0044-4df5-b82e-da66317591f7" width="50%"/>

#### 구현 이유
- 멀티 대전의 전투 시스템 구현을 위해

#### 구현 방법
- Weapon Collider를 활성화하는 함수 작성
<img src="https://github.com/user-attachments/assets/30d10b39-db25-4103-8f9e-aacb0703196f" width="50%"/>
<br/>
<br/>

```C#
public void AttackColliderActive(float time)
{
	for (int i = 0; i < _weaponColliders.Length; i++)
	{
	    _weaponColliders[i].enabled = true;
	}
	
	StartCoroutine(COAttackColliderInactive(time));
}

private IEnumerator COAttackColliderInactive(float time)
{
	yield return new WaitForSeconds(time);
	
	for (int i = 0; i < _weaponColliders.Length; i++)
	{
	    _weaponColliders[i].enabled = false;
	}
}
```
<br/>

- Animation Events 등록
<img src="https://github.com/user-attachments/assets/04cffd69-7ba3-4f76-85e7-ee4d40f5b176" width="50%"/>
<br/>
<br/>

- OnPhotonSerializeView 함수를 통해, 능력치 데이터 송수신
```C#
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    // 데이터 보내기 (isMine == true)
    if (stream.IsWriting)
    {
        stream.SendNext(GameManager.I.DataManager.PlayerData.Atk);
        stream.SendNext(GameManager.I.DataManager.PlayerData.SkillAtk);
        stream.SendNext(GameManager.I.DataManager.PlayerData.Def);
    }
    // 데이터 받기 (isMine == false)
    else
    {
        Atk = (float)stream.ReceiveNext();
        SkillAtk = (float)stream.ReceiveNext();
        Def = (float)stream.ReceiveNext();
    }
}
``` 
​<br/>

- RPC를 통해, 공격 애니메이션 실행
```C#
if (PhotonView.IsMine) PhotonView.RPC("PlayerAttackRPC", RpcTarget.AllViaServer);

[PunRPC]
public void PlayerAttackRPC()
{
    _anim.SetTrigger("Attack");
}
```
<br/>

- RPC를 통해, 넉백 구현
```C#
private void OnTriggerEnter(Collider other)
{
    if (other.gameObject.CompareTag("Player")
    {
	if (!_photonView.IsMine)
	{
	    _atk = _playerCharacter.Atk;
	    other.GetComponent<PlayerCharacter>().PhotonView.RPC("RPCPlayerNuckback", RpcTarget.AllViaServer, _playerCharacter.transform.position, _atk);
	}
    }
}

[PunRPC]
public void RPCPlayerNuckback(Vector3 attackPosition, float power)
{
    if (PhotonView.IsMine) PhotonView.RPC("PlayerHitRPC", RpcTarget.AllViaServer);
    Vector3 dir = (transform.position - attackPosition).normalized;

    if (PhotonView.IsMine) _rigidbody.velocity = new Vector3(dir.x, 0, dir.z) * (power - _playerData.Def);
    else _rigidbody.velocity = new Vector3(dir.x, 0, dir.z) * (power - Def);

    transform.LookAt(attackPosition);
}

[PunRPC]
public void PlayerHitRPC()
{
    _anim.SetTrigger("Hit");
}
```
<br/>

### 5. 랭킹 구현
<img src="https://github.com/user-attachments/assets/cfbc5009-0507-46f7-a827-3ff786e20206" width="50%"/>

#### 구현 이유
- 경쟁 심리를 이용해서 유저들이 더 게임을 플레이 하도록 하기 위해

#### 구현 방법
- 뒤끝 서버 설치 및 서버 접속
```C#
private void BackendSetup()
{
BackendReturnObject bro = Backend.Initialize(true);

if (bro.IsSuccess())
{
    Debug.Log("뒤끝 서버 연동 성공 : " + bro); // 성공일 경우 statusCode 204 Success
}
else
{
    Debug.LogError("뒤끝 서버 연동 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
}
}
```
<br/>

- 뒤끝 서버에서 비교할 데이터의 데이터 테이블 생성
<img src="https://github.com/user-attachments/assets/e9fc4d4c-2d22-4af5-bf05-ffef142f4600" width="50%"/>
<br/>
<br/>

```C#
// 데이터 테이블에 추가하는 함수
public void InsertData()
{
    Param param = GetUserDataParam();
    BackendReturnObject bro = Backend.GameData.Insert("USER_DATA", param);

    if (bro.IsSuccess())
    {
        Debug.Log("데이터 추가를 성공했습니다");
    }
    else
    {
        Debug.Log("데이터 추가를 실패했습니다");
    }
}

// Param : 데이터를 송수신할 때 사용하는 class
private Param GetUserDataParam()
{
    Param param = new Param();
    param.Add("RankPoint", GameManager.I.DataManager.GameData.RankPoint);
    return param;
}
```
<br/>

- 뒤끝 서버 랭킹 추가
<img src="https://github.com/user-attachments/assets/eca92b5d-5868-4f2e-ba13-717a0060c88d" width="50%"/>
<br/>
<br/>

- 랭킹 데이터 갱신
```C#
// 데이터 테이블에 추가하는 함수
private void UpdateMyRankData(int value)
{
	string rowInDate = string.Empty;

	// 랭킹 데이터를 업데이트하려면 게임 데이터에서 사용하는 데이터의 inDate 값 필요
	BackendReturnObject bro = Backend.GameData.GetMyData("USER_DATA", new Where());
	
	if (!bro.IsSuccess())
	{
	    Debug.LogError("랭킹 업데이트를 위한 데이터 조회 중 문제가 발생했습니다.");
	    return;
	}
	
	Debug.Log("랭킹 업데이트를 위한 데이터 조회에 성공했습니다.");
	
	if(bro.FlattenRows().Count > 0)
	{
	    rowInDate = bro.FlattenRows()[0]["inDate"].ToString();
	}
	else
	{
	    Debug.LogError("데이터가 존재하지 않습니다.");
	}
	
	Param param = new Param()
	{
	    {"RankPoint",  value}
	};
	
	// 해당 데이터테이블의 데이터를 갱신하고, 랭킹 데이터 정보 갱신
	bro = Backend.URank.User.UpdateUserScore(RANK_UUID, "USER_DATA", rowInDate, param);
	
	if(bro.IsSuccess())
	{
	    Debug.Log("랭킹 등록에 성공했습니다.");
	}
	else
	{
	    Debug.LogError("랭킹 등록에 실패했습니다.");
	}
}
```
<br/>

- 뒤끝 서버 Json 데이터를 파싱해서 나의 랭킹 불러오기
```C#
public void GetMyRank()
{
    // 내 랭킹 정보 불러오기
    BackendReturnObject bro = Backend.URank.User.GetMyRank(RANK_UUID);

    if(bro.IsSuccess())
    {
        try
        {
            JsonData rankDataJson = bro.FlattenRows();

            // 받아온 데이터의 개수가 0 -> 데이터가 없음
            if (rankDataJson.Count <= 0)
            {
                Debug.Log("나의 랭킹 데이터가 존재하지 않습니다.");
            }
            else
            {
                _rankPoint = int.Parse(rankDataJson[0]["score"].ToString());
                _rank = int.Parse(rankDataJson[0]["rank"].ToString());
                _userName = rankDataJson[0]["nickname"].ToString();
            }
        }
        // 나의 랭킹 정보 JSON 데이터 파싱에 실패했을 때
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
    else
    {
        // 나의 랭킹 정보를 불러오는데 실패했을 때
    }
}
```
<br/>

- 뒤끝 서버 Json 데이터를 파싱해서 유저 랭킹 불러오기
```C#
private const int MAX_RANK_LIST = 10;

public void GetRankList()
{
    // 랭킹 테이블에 있는 유저의 offset ~ offset + limit 순위 랭킹 정보를 불러옴
    BackendReturnObject bro = Backend.URank.User.GetRankList(RANK_UUID, MAX_RANK_LIST, 0);

    if(bro.IsSuccess())
    {
        // JSON 데이터 파싱 성공
        try
        {
            Debug.Log("랭킹 조회에 성공했습니다.");
            JsonData rankDataJson = bro.FlattenRows();

            // 받아온 데이터의 개수가 0 -> 데이터가 없음
            if(rankDataJson.Count <= 0)
            {
                Debug.Log("랭킹 데이터가 존재하지 않습니다.");
            }
            else
            {
                int rankCount = rankDataJson.Count;

                // 받아온 rank 데이터의 숫자만큼 데이터 출력
                for (int i = 0; i < rankCount; i++)
                {
                    _rankPoint = int.Parse(rankDataJson[i]["score"].ToString());
                    _rank = int.Parse(rankDataJson[i]["rank"].ToString());
                    _userName = rankDataJson[i]["nickname"].ToString();
                }
                // rankCount가 Max값만큼 존재하지 않을 때, 나머지 랭킹
                for (int i = rankCount; i < MAX_RANK_LIST; i++)
                {
                    // 랭킹 데이터 비활성화
                }
            }
        }
        // JSON 데이터 파싱 실패
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }
    else
    {
        Debug.LogError("랭킹 조회에 실패했습니다.");
    }
}
```
<br/>

### 6. Admob 광고 구현
<img src="https://github.com/user-attachments/assets/cee62c9f-3df8-4753-bbd6-969521b3afab" width="50%"/> 

#### 구현 이유
- 유저들이 광고를 시청하면 Coin을 얻게하기 위해
- 유저들이 광고를 시청함으로써, 게임의 수익화를 실현하기 위해

#### 구현 방법
- Google Admob에서 보상형 광고와 배너 광고 생성
<img src="https://github.com/user-attachments/assets/b98f1d69-bf1b-4164-8eab-1878be77beb0" width="50%"/>
<br/>
<br/>

- Unity plugin을 설치 후, 프로젝트에 Import
- 테스트 ID와 광고 ID를 적용해서 스크립트 작성

```C#
public void Init()
{
	if (IsTestMode)
	{
	    // 테스트용 ID
	    _adRewardUnitId = "ca-app-pub-3940256099942544/5224354917";
	    _adBannerUnitId = "ca-app-pub-3940256099942544/6300978111";
	}
	else
	{
	    #if UNITY_ANDROID
	    // 광고 ID
	    _adRewardUnitId = "";
	    _adBannerUnitId = "";
	    #elif UNITY_IPHONE
	    // 테스트용 ID
	    _adRewardUnitId = "ca-app-pub-3940256099942544/1712485313";
	    _adBannerUnitId = "ca-app-pub-3940256099942544/2934735716";
	    #else
	    _adRewardUnitId = "unused";
	    _adBannerUnitId = "unused";
	    #endif
	}

	MobileAds.Initialize((InitializationStatus initStatus) => { });
}

//보상형 광고 로드, 사용 시 호출
public void LoadRewardedAd()
{
	if (_rewardedAd != null)
	{
	    _rewardedAd.Destroy();
	    _rewardedAd = null;
	}

	var adRequest = new AdRequest();

	RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
	{
		if (error != null || ad == null)
		{
		    Debug.LogError("Rewarded ad failed to load an ad " +
				   "with error : " + error);
		    return;
		}

		Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

		_rewardedAd = ad;
		RegisterEventHandlers(_rewardedAd);
		ShowRewardedAd();
	});
}

public void ShowRewardedAd()
{
	if (_rewardedAd != null && _rewardedAd.CanShowAd())
	{
	    _rewardedAd.Show((Reward reward) =>
	    {
		// 광고 보상 입력
	    });
	}
}

private void RegisterEventHandlers(RewardedAd ad)
{
	ad.OnAdPaid += (AdValue adValue) => { };
	ad.OnAdImpressionRecorded += () => { };
	ad.OnAdClicked += () => { };
	ad.OnAdFullScreenContentOpened += () => { };
	ad.OnAdFullScreenContentClosed += () => { }; // 광고 창을 닫을 때, 실행할 내용
	// 광고 불러오기를 실패했을 때
	ad.OnAdFullScreenContentFailed += (AdError error) =>
	{
	    LoadRewardedAd();
	};
}

//배너 광고 로드, 사용 시 호출
public void LoadBannerAd()
{
	if (_bannerView == null)
	{
	    CreateBannerView();
	}
	
	var adRequest = new AdRequest();
	_bannerView.LoadAd(adRequest);
}

//배너 광고 보여주기
private void CreateBannerView()
{
	if (_bannerView != null)
	{
	    DestroyAd();
	}
	
	_bannerView = new BannerView(_adBannerUnitId, AdSize.Banner, AdPosition.Top);
}

//배너 광고 제거
public void DestroyAd()
{
	if (_bannerView != null)
	{
	    _bannerView.Destroy();
	    _bannerView = null;
	}
}
```
<br/>

### 7. Enemy 상태 패턴 구현
<img src="https://github.com/user-attachments/assets/3b2e2a93-5a9b-42e1-9e36-638c8fd8ec4c" width="50%"/>

#### 구현 이유
- 다양한 상태를 가진 Enemy 움직임 구현
- 끊임없이 독립적으로 행동해야 함
- 유연한 상태 관리로 필요에 따라 상태를 추가하거나 수정이 가능해야 함

#### 구현 방법
- IState 인터페이스 : 구체적인 상태 클래스로 연결할 수 있도록 설정
```C#
public interface IEnemyState
{
    void Handle(EnemyController controller);
}
``` 
​
- Context 스크립트 : 클라이언트가 객체의 내부 상태를 변경할 수 있도록 요청하는 인터페이스를 정의
```C#
public void Transition()
{
    CurrentState.Handle(_enemyController);
}

public void Transition(IEnemyState state)
{
    CurrentState = state;
    CurrentState.Handle(_enemyController);
}
```
​
- EnemyController 스크립트 : 각 State 컴포넌트 연결, State 실행
```C#

// Start문과 동일하게 사용
private void Start()
{
	_enemyStateContext = new EnemyStateContext(this);
	_walkState = gameObject.AddComponent<EnemyWalkState>();
}

public void WalkStart()
{
	_enemyStateContext.Transition(_walkState);
}
```
<br/>
<br/>

- State 스크립트 : 각 State를 정의, State 변경 조건 설정
<img src="https://github.com/user-attachments/assets/b85edb66-b5ad-4c2b-b50e-de1237b26c55" width="50%"/>
<br/>
<br/>

```C#
// Start문과 동일하게 사용
public void Handle(EnemyController enemyController)
{
	if (!_enemyController)
	    _enemyController = enemyController;
	
	Debug.Log("Walk 상태 시작");
	StartCoroutine(COUpdate());
}

// Update문과 동일하게 사용
private IEnumerator COUpdate()
{
	while (true)
	{
	    _dir = (_enemyController.Target.transform.position - transform.position).normalized;
	    transform.position += _dir * _enemyController.Speed * Time.deltaTime;
	
	    if (_enemyController.CheckPlayer())
	    {
		_enemyController.AttackStart();
		_enemyController.EnemyAnimator.SetBool("Attack", true);
		break;
	    }
	
	    if (_enemyController.IsHit_attack || _enemyController.IsHit_skill)
	    {
		_enemyController.HitStart();
		_enemyController.EnemyAnimator.SetTrigger("Hit");
		break;
	    }
		
	    yield return null;
	}
}
```
<br/>

### 8. 로딩 씬 구현
<img src="https://github.com/user-attachments/assets/a26ffcc9-fdc0-4628-ba8f-952d1d6d90ba" width="50%"/>  

#### 구현 이유
- 씬이 전환 될 때, 다음 씬에서 사용될 리소스들을 읽어와서 게임을 위한 준비 작업 필요
- 로딩 화면이 없다면 가만히 멈춘 화면이나 까만 화면만 보일 수 있음
- 씬이 전환 될 때, 지루한 대기 시간을 지루하지 않게 하기 위해

#### 구현 방법
- 씬을 불러오는 도중에 다른 작업이 가능 비동기 방식 씬 전환 구현
```C#
IEnumerator LoadScene()
{
    yield return null;
    AsyncOperation op = SceneManager.LoadSceneAsync(NextScene);
    op.allowSceneActivation = false;
    float timer = 0.0f;
    while (!op.isDone)
    {
        yield return null;
        timer += Time.deltaTime;
        if (op.progress < 0.9f)
        {
            _loadingBar.value = Mathf.Lerp(_loadingBar.value, op.progress, timer);
            if (_loadingBar.value >= op.progress)
            {
                timer = 0f;
            }
        }
        else
        {
            _loadingBar.value = Mathf.Lerp(_loadingBar.value, 1f, timer);
            if (_loadingBar.value == 1.0f)
            {
                op.allowSceneActivation = true;
                yield break;
            }
        }
    }
}
```

- 리소스 로딩이 끝나기 전에 씬 로딩 되는 것을 막기 위해 allowSceneActivation을 false로 설정
- allowSceneActivation을 false로 90% 로드 한 상태로 대기하고, true 변경 시, 남은 부분을 로드하고 씬 이동
<br/>

## 💥 트러블 슈팅

### 1. PVP 구현
#### 문제 상황
- 다른 클라이언트와 연동 가능한 서버가 필요

#### 해결 방안
##### Photon PUN2 사용
- 참고할 자료 및 내용이 많이 공유되어 있음
- 많은 개발자들이 대표적으로 가장 많이 사용
- 무료 버전으로도 비교적 많은 인원을 수용할 수 있음
- Shared 네트워크 구조 방식만 제공
##### Photon Fusion2 사용
- 많은 인원을 수용할 수 있음
- 직관적이고 간단하게 변수 동기화 가능
- 여러가지 네트워크 구조 방식 제공하고 네트워크 지연 보간 기능 제공
- 기능과 성능이 우수함
- 비교적 어려운 사용 방법
##### 서버 직접 개발
- 직접 게임 특성에 맞게 서버를 개발 가능
- 서버를 직접 개발하기에는 많은 시간과 노력이 필요
 
#### 의견 결정
##### Photon PUN2 사용
- 1:1 PVP 게임이므로, 많은 인원을 수용할 필요 없음
- 멀티 게임 개발 경험이 없기 때문에 많은 참고할 자료 및 내용이 필요
- 무료 버전으로도 충분히 기획한 게임 구현 가능
- 클라이언트 개발자로서 서버를 직접 개발할 필요성을 느끼지 못함
<br/>

### 2. PUN2 Transform 동기화
<img src="https://github.com/user-attachments/assets/f8dedc98-a67c-41a0-892b-8849f21cc587" width="50%"/>
<br/>
<br/>

#### PhotonTransformView 컴포넌트로 동기화
<img src="https://github.com/user-attachments/assets/ec1c8a19-9eda-4746-bbc6-4e96269e4043" width="50%"/>
<br/>
<br/>

- 간단하고 직관적으로 Position, Rotation 동기화 가능
- 끊김 현상, 딜레이가 심하게 발생
- 점프 시, Position Y 값을 제대로 동기화하지 못함
- 유니티 3D의 빠른 움직임을 동기화 할때는 적합하지 않음

#### OnPhotonSerializeView 함수를 통해 Transform 데이터 실시간 송수신으로 개선
- 실시간으로 전달된 데이터를 통해 각각 클라이언트에서 직접 움직임을 실행
```C#
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    // 데이터 보내기 (isMine == true)
    if (stream.IsWriting)
    {
        stream.SendNext(transform.position);
        stream.SendNext(transform.rotation);
    }
    // 데이터 받기 (isMine == false)
    else
    {
        _playerPosition = (Vector3)stream.ReceiveNext();
        _playerRotation = (Quaternion)stream.ReceiveNext();
    }
}
```
<br/>

- OnPhotonSerializeView 호출 빈도를 직접 설정
```C#
private void Awake()
{
    PhotonNetwork.SendRate = 60;
}
```
<br/>

#### 결과
<img src="https://github.com/user-attachments/assets/c0625e71-1016-48cc-8893-512b0c9db764" width="50%"/>
<br/>
<br/>

- 끊김 현상, 딜레이 개선
- 점프 시, Position Y 값을 제대로 동기화하지 못하는 현상 해결
<br/>

### 3. 랭킹 구현을 위한 서버 선택
#### 문제 상황
- 랭킹 시스템에 사용할 서버 필요

#### 해결 방안
##### 뒤끝 서버 사용
- 이미 랭킹 시스템이 구현되어 있음
- 참고 가능한 자료, 정보가 비교적 많음
- 일정 사용량 초과 시, 발생하는 사용료가 타 서버에 비해서 비쌈
##### Firebase 서버 사용
- 매우 저렴한 비용
- 빠른 속도
- 직관적인 코드로 쉽게 사용 가능
##### 서버 직접 개발
- 직접 게임 특성에 맞게 서버를 개발 가능
- 서버를 직접 개발하기에는 많은 시간과 노력이 필요
 
#### 의견 결정
##### 뒤끝 서버 사용
<img src="https://github.com/user-attachments/assets/cd2b2bc5-b430-4ebd-8731-a8660d90513c" width="50%"/>
<br/>
<br/>

- 이미 데이터를 비교해서 순위를 결정하는 랭킹 시스템이 구현되어 있기 때문에 사용 방법만 익히면 됨
- 멀티 구현이 미숙하기 때문에 참고 가능한 자료, 정보가 많은 뒤끝 서버로 구현하는 것이 좋다고 판단
- 랭킹 시스템만 구현하고 사용하는 유저가 적기 때문에 무료 버전의 사용량으로도 충분하다고 판단
<br/>

### 4. List 데이터 수정 시, 원본 데이터도 수정
#### 문제 상황
```C#
public class CharacterData
{
    public string Tag;
    public bool IsEquip;
    public int Level;
    public float Speed;
    public float Atk;
    public float Def;
}

public List<CharacterData> CharacterInventory;
public CharacterData[] CharacterDatas;

if (!CharacterIsGet(_data)) 
{
	GameManager.I.DataManager.DataWrapper.CharacterInventory.Add(_data);
}
```
<br/>

- CharacterData의 초기 데이터를 CharacterDatas 배열에서 관리
- 캐릭터를 얻게 되면 해당 CharacterData를 CharacterInventory List에 추가
- CharacterInventory List의 데이터가 수정되면, CharacterDatas 배열의 데이터도 함께 변경됨
- CharacterData가 class이기 때문에 Heap 영역에 할당되고, 참조 형식이기 때문에 원본 데이터도 함께 변경

#### 해결 방안
##### struct 사용
- struct는 stack 영역에 할당되고, 값 형식이기 때문에 근본적인 해결 가능
- 현재 구현한 데이터 저장 방식이 class 형식만 저장 가능하기 때문에 데이터 저장 방식 변경 필요
##### 별도의 인벤토리 List를 사용하지 않기
- List를 사용하지 않고 각각 데이터마다 IsGet이라는 bool 값을 설정
- 매번 CharacterDatas 배열 전체를 순회하여 캐릭터를 가지고 있는지 판단하기 때문에 비효율적이라고 판단
##### class를 참조하지 않고 값 형식 복사
- class를 값 형식으로 복사하는 깊은 복사 구현
- 객체의 내부까지 모두 복사하는 복잡한 깊은 복사를 굳이 구현하는 것은 비효율적이라고 판단
##### class에 수정하지 않을 원본 값을 추가
- class에 별도의 원본 데이터를 추가
 
#### 의견 결정
##### class에 수정하지 않을 원본 값을 추가
- 변경하지 않을 별도의 원본 데이터를 추가
- 근본적인 해결 방법은 아니지만, 가장 합리적인 해결 방법이라고 판단
```C#
public class CharacterData
{
    public string Tag;
    public bool IsEquip;
    public int Level;
    public float Speed;
    public float Atk;
    public float Def;
    public float OriginSpeed;
    public float OriginAtk;
    public float OriginDef;
}
```
<br/>

### 2. ObjectPool 사용 시, OnEnable문으로 오브젝트 초기화
<img src="https://github.com/JaeMinNa/CastleDefence2D/assets/149379194/12ec91d2-b3d9-485b-aa63-565721640b80" width="50%"/>

#### Start문 사용
- ObjectPool로 재사용할 때, 정상적으로 동작하지 않음
- Start문의 내용이 재실행되지 않음
- 오브젝트 활성화 될 때 마다, 초기화 해야함
 
```C#
private void Start()
{
	_enemyStateContext = new EnemyStateContext(this);
	
	_walkState = gameObject.AddComponent<EnemyWalkState>();
	_hitState = gameObject.AddComponent<EnemyHitState>();
	_attackState = gameObject.AddComponent<EnemyAttackState>();
	Animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
	Rigdbody = GetComponent<Rigidbody2D>();
	
	Hp = EnemyData.Hp;
	Ishit = false;
	IsAttack = false;
	
	_enemyStateContext.Transition(_walkState);
}
```

#### OnEnable문 사용
- 오브젝트 활성화 시, Start문 내용은 실행되지 않고, 최초 1회만 실행
- OnEnable문 -> Start문 순으로 실행

```C#
private void Start()
{
	_enemyStateContext = new EnemyStateContext(this);
	
	_walkState = gameObject.AddComponent<EnemyWalkState>();
	_hitState = gameObject.AddComponent<EnemyHitState>();
	_attackState = gameObject.AddComponent<EnemyAttackState>();
	Animator = gameObject.transform.GetChild(0).GetComponent<Animator>();
	Rigdbody = GetComponent<Rigidbody2D>();
	
	Hp = EnemyData.Hp;
	Ishit = false;
	IsAttack = false;
	
	_enemyStateContext.Transition(_walkState);
}

private void OnEnable()
{
	if(_enemyStateContext != null)
	{
	    Hp = EnemyData.Hp;
	    _enemyStateContext.Transition(_walkState);
	}
}
```

#### 결과
- ObjectPool로 프리팹의 재사용 시, 오브젝트가 활성화 될 때마다 코드를 실행 가능
<img src="https://github.com/JaeMinNa/CastleDefence2D/assets/149379194/f676082d-c675-4104-98fb-fbc3d7bd8715" width="50%"/> 
<br/>
<br/>

### 3. 상태 패턴을 이용한 Enemy와 Player 구현
<p align="center">
  <img src="https://github.com/JaeMinNa/CastleDefence2D/assets/149379194/c6f239cc-d98a-4c89-ba16-bc3895f15e25" width="49%"/>
  <img src="https://github.com/JaeMinNa/CastleDefence2D/assets/149379194/defa1871-065a-41ab-8ef7-40c0c030f808" width="49%"/>
</p>

#### 문제 상황
- 적과 동료의 독립적인 움직임을 구현하기 위한 방법이 필요

#### 해결 방안
##### 조건문과 스위치문 사용
- 간단하고 직관적으로 구현 가능
- 행동이 많다면 코드가 복잡해짐
##### 상태 패턴
- 새로운 상태 추가가 쉬움
- 확장성이 용이
  
#### 의견 결정
##### 상태 패턴으로 구현
- 특정 조건에 따라 각각 다른 행동을 할 수 있음
- 특정 행동을 추가해도 유지 관리가 용이
<br/>

### 4. Physics2D.OverlapCircleAll를 이용한 Targetting 구현
<img src="https://github.com/JaeMinNa/CastleDefence2D/assets/149379194/58f3acf1-ea78-4e77-9a3f-1259477a4fab" width="50%"/>

#### 문제 상황
- Player의 Skill 사용 시, 데미지 적용을 위한 적 Targetting 방법이 필요

#### 해결 방안
##### BoxCollider로 IsTrigger 범위 설정
- 간단하게 구현 가능
##### Physics2D.OverlapCircleAll를 사용
<p align="center">
<img src="https://github.com/JaeMinNa/CastleDefence2D/assets/149379194/cd7d06f1-4216-4029-9536-417654b3d5be" width="49%"/>
<img src="https://github.com/JaeMinNa/CastleDefence2D/assets/149379194/e32cf54c-c21c-408f-9485-7dbeb673d876" width="49%"/>
</p>

- 특정 범위 내의 적이나 동료 판별 가능

```C#
private void Targetting()
{
int layerMask = (1 << _layerMask);  // Layer 설정
_targets = Physics2D.OverlapCircleAll(transform.position, 3f, layerMask);

// 데미지 적용

}
```
 
#### 의견 결정
##### Physics2D.OverlapCircleAll로 구현
- BoxCollider 사용 시, 다른 Collider나 Raycast와 충돌할 위험이 있음
- Skill이 적과 충돌할 때, 순간적으로 적들을 인식 가능
<br/>

### 5. 적과 적의 충돌
<img src="https://github.com/JaeMinNa/CastleDefence2D/assets/149379194/799db977-9173-4d6b-bd4c-66529a7912cd" width="50%"/>

#### 문제 상황
- 적과 적이 충돌하지 않는 방법 필요

#### 해결 방안
##### Layer Collision Matrix 설정
- Project Settings - Physics2D 에서 간단하게 설정 가능
<img src="https://github.com/JaeMinNa/CastleDefence2D/assets/149379194/fe62ae49-be60-4425-b099-1541ffd523ee" width="50%"/>

##### Collider의 IsTrigger 사용
- 간단하게 설정가능
- 하지만 땅을 통과하기 때문에 추가 Collider가 필요

#### 의견 결정
##### Layer Collision Matrix 설정
- 적에게 추가 Collider를 생성하면 적을 두 번 인식할 수도 있음
- 유니티 자체 기능으로 간편하게 설정 가능
<br/>

### 6. 데이터 저장 방법

#### 문제 상황
- 기존의 ScriptableObject로 저장된 데이터는 유니티 에디터에서만 저장
- 빌드 후, ScriptableObject로 데이터를 저장할 수 없기 때문에 다른 데이터를 저장할 방법 필요

#### 해결 방안
##### EasySave 에셋 사용
- 유니티 에셋스토어의 검증된 에셋으로, 간편하고 기능이 많음
- 유료로 다운 받을 수 있음

##### Json 사용
- 텍스트 기반의 데이터 형식
- 유니티에서 JSON Utility 클래스를 사용해서 오브젝트 데이터를 쉽게 다룰 수 있음
- 데이터를 저장하거나 교환하는데 자주 사용되는 경량의 데이터 교환 형식
- 키-값 쌍으로 이루어진 데이터 객체와 배열을 포함

##### PlayerPrefs 사용
- 가장 간단하게 저장할 수 있는 유니티 자체 기능
- GameObject 데이터 저장하기는 어려움

#### 의견 결정
##### Json 사용
- 에셋을 구매하는 것보다, 직접 기능을 구현하고 싶었음
- PlayerPrefs의 데이터 저장으로 인벤토리의 Skill을 저장하는 것이 어렵다고 판단
- 구현 난이도가 비교적 쉬움
<br/>

## 📋 프로젝트 회고
### 잘한 점
 - 초기 계획대로 구글 플레이 스토어에 안드로이드 출시 완료
 - WebGL 빌드 후, Itch.io 업로드 완료
 - Admob 보상형 광고 적용 완료
 - Json 데이터 저장 기능 구현
 - 초기 기획과 크게 벗어나지 않게 게임 개발 성공
 - 전체적으로 이전 프로젝트에 비해서 최적화에 신경을 많이 씀
 - 기획부터 최종 개발까지 전부 혼자서 진행
<br/>

### 한계
- iOS 빌드에 대한 공부가 더 필요
- 장르의 특성 상, 다양한 컨텐츠가 부족
- 출시 후, 홍보 및 광고의 한계
- 수익화를 실현했지만, 실제 수익을 기대하기는 힘듦
- 목표 기간에 맞추지 못함
<br/>

### 소감
처음으로 기획부터 최종 개발까지 혼자서 진행한 프로젝트였습니다. 초기 계획대로 구글 플레이 스토어에 안드로이드 출시를 처음으로 성공했습니다. 직전 프로젝트에서 최적화 부분이 많이 부족하다고 느껴서, ObjectPool을 사용한 최적화에 신경을 많이 쓰고 적용했습니다. 게임 개발까지는 이전 프로젝트의 경험을 바탕으로 빠르게 할 수 있었지만, 빌드, 광고 적용 및 출시에서 생각보다 시간을 많이 소요했습니다. 하지만, 개인 블로그에 잘 정리를 했기 때문에, 다음 프로젝트에서는 더욱 빠르게 진행할 수 있을 것 같습니다. 그리고 출시를 하고 끝이 아닌, 수익화를 실현할 수 있는 광고나 홍보, 광고 보상 등이 정말 중요하다고 느낄 수 있었던 프로젝트였습니다.
  
