/**************************************
	Copyright 2015 Unluck Software
	www.chemicalbliss.com
***************************************/
#pragma strict
#pragma downcast
@HideInInspector
public var _spawner:SchoolController;
private var _wayPoint : Vector3;
@HideInInspector
public var _speed:float= 10;				//Fish Speed
private var _stuckCounter:float;			//prevents looping around a waypoint
private var _damping:float;					//Turn speed
private var _model:Transform;				//Model with animations
private var _targetSpeed:float;				//Fish target speed
private var tParam : float = 0;				//
private var _rotateCounterR:float;			//Used to increase avoidance speed over time
private var _rotateCounterL:float;			
public var _scanner:Transform;				//Scanner object used for push, this rotates to check for collisions
private var _scan:boolean = true;			
private var _instantiated:boolean;			//Has this been instantiated
private static var _updateNextSeed:int = 0;	//When using frameskip seed will prevent calculations for all fish to be on the same frame
private var _updateSeed:int = -1;
@HideInInspector
public var _cacheTransform:Transform;

#if UNITY_EDITOR
static var _sWarning:boolean;
#endif

function Start(){
	//Check if there is a controller attached
	if(!_cacheTransform) _cacheTransform = transform;
	if(_spawner){
		SetRandomScale();
		LocateRequiredChildren();
		RandomizeStartAnimationFrame();
		SkewModelForLessUniformedMovement();
		_speed = Random.Range(_spawner._minSpeed, _spawner._maxSpeed);
		Wander(0);
		SetRandomWaypoint();
		CheckForBubblesThenInvoke();
		_instantiated = true;
		GetStartPos();
		FrameSkipSeedInit();
		_spawner._activeChildren++;
		return;
	}
	
	this.enabled = false;
	Debug.Log(gameObject + " found no school to swim in: " + this + " disabled... Standalone fish not supported, please use the SchoolController"); 
}

function Update() {
	//Skip frames
	if (_spawner._updateDivisor <=1 || _spawner._updateCounter == _updateSeed){
		CheckForDistanceToWaypoint();
		RotationBasedOnWaypointOrAvoidance();
		ForwardMovement();
		RayCastToPushAwayFromObstacles();
		SetAnimationSpeed();
	}
}

function FrameSkipSeedInit(){
	if(_spawner._updateDivisor > 1){
		var _updateSeedCap:int = _spawner._updateDivisor -1;
		_updateNextSeed++;
		this._updateSeed = _updateNextSeed;
		_updateNextSeed = _updateNextSeed % _updateSeedCap;
	}
}

function CheckForBubblesThenInvoke() {
	if(_spawner._bubbles)
		InvokeRepeating("EmitBubbles", (_spawner._bubbles._emitEverySecond*Random.value)+1 , _spawner._bubbles._emitEverySecond);	
}

function EmitBubbles(){
	_spawner._bubbles.EmitBubbles(_cacheTransform.position, _speed);
}

function OnDisable() {
	CancelInvoke();
	_spawner._activeChildren--;
}

function OnEnable() {
	if(_instantiated){
		CheckForBubblesThenInvoke();
		_spawner._activeChildren++;
	}
}

function LocateRequiredChildren(){
	if(!_model) _model = _cacheTransform.Find("Model");
	if(!_scanner){
		_scanner = new GameObject().transform;
		_scanner.parent = this.transform;
		_scanner.localRotation = Quaternion.identity;
		_scanner.localPosition = Vector3.zero;
		#if UNITY_EDITOR
		if(!_sWarning){
			Debug.Log("No scanner assigned: creating... (Increase instantiate performance by manually creating a scanner object)");
			_sWarning = true;
		}
		#endif
	}
}

function SkewModelForLessUniformedMovement () {
	// Adds a slight rotation to the model so that the fish get a little less uniformed movement
	var rx:Quaternion;
	rx.eulerAngles = Vector3(0, 0 , Random.Range(-25, 25));
	_model.	rotation = rx;
}

function SetRandomScale(){
	var sc:float = Random.Range(_spawner._minScale, _spawner._maxScale);
	_cacheTransform.localScale=Vector3.one*sc;
}

function RandomizeStartAnimationFrame(){
	for (var state : AnimationState in _model.GetComponent.<Animation>()) {
		state.time = Random.value * state.length;
	}
}

function GetStartPos(){
	//-Vector is to avoid zero rotation warning
	_cacheTransform.position = _wayPoint - Vector3(.1,.1,.1);
}

function findWaypoint():Vector3{
	var t:Vector3;
	t.x = Random.Range(-_spawner._spawnSphere, _spawner._spawnSphere) + _spawner._posBuffer.x;
	t.z = Random.Range(-_spawner._spawnSphereDepth, _spawner._spawnSphereDepth) + _spawner._posBuffer.z;
	t.y = Random.Range(-_spawner._spawnSphereHeight, _spawner._spawnSphereHeight) + _spawner._posBuffer.y;
	return t;
}

//Uses scanner to push away from obstacles
function RayCastToPushAwayFromObstacles() {
	if(_spawner._push){
		RotateScanner();
		RayCastToPushAwayFromObstaclesCheckForCollision();
	}
}

function RayCastToPushAwayFromObstaclesCheckForCollision () {
	var hit : RaycastHit;
	var d:float;
	var cacheForward:Vector3 = _scanner.forward;
	if (Physics.Raycast(_cacheTransform.position, cacheForward, hit, _spawner._pushDistance, _spawner._avoidanceMask)){
		var s:SchoolChild;
		s = hit.transform.GetComponent(SchoolChild);
		d = (_spawner._pushDistance - hit.distance)/_spawner._pushDistance;// Equals zero to one. One is close, zero is far
		if(s){
			_cacheTransform.position -= cacheForward*_spawner._newDelta*d*_spawner._pushForce;
		}
		else{
			_speed -= .01*_spawner._newDelta;
			if(_speed < .1)
			_speed = .1;
			_cacheTransform.position -= cacheForward*_spawner._newDelta*d*_spawner._pushForce*2;
			//Tell scanner to rotate slowly
			_scan = false;
		}
	}else{
		//Tell scanner to rotate randomly
		_scan = true;
	}
}

function RotateScanner() {
	//Scan random if not pushing
	if(_scan){
		_scanner.rotation = Random.rotation;
		return;
	}
	//Scan slow if pushing
	_scanner.Rotate(Vector3(150*_spawner._newDelta,0,0));
}

function Avoidance ():boolean {
	//Avoidance () - Returns true if there is an obstacle in the way
	if(!_spawner._avoidance)
		return false;
	var hit : RaycastHit;
	var d:float;
	var rx:Quaternion = _cacheTransform.rotation;
	var ex:Vector3 = _cacheTransform.rotation.eulerAngles;
	var cacheForward:Vector3 = _cacheTransform.forward;
	var cacheRight:Vector3 = _cacheTransform.right;
	//Up / Down avoidance
	if (Physics.Raycast(_cacheTransform.position, -Vector3.up+(cacheForward*.1), hit, _spawner._avoidDistance, _spawner._avoidanceMask)){
		//Debug.DrawLine(_cacheTransform.position,hit.point);
		d = (_spawner._avoidDistance - hit.distance)/_spawner._avoidDistance;
		ex.x -= _spawner._avoidSpeed*d*_spawner._newDelta*(_speed +1);
		rx.eulerAngles = ex;
		_cacheTransform.rotation = rx;
	}
	if (Physics.Raycast(_cacheTransform.position, Vector3.up+(cacheForward*.1), hit, _spawner._avoidDistance, _spawner._avoidanceMask)){
		//Debug.DrawLine(_cacheTransform.position,hit.point);
		d = (_spawner._avoidDistance - hit.distance)/_spawner._avoidDistance;
		ex.x += _spawner._avoidSpeed*d*_spawner._newDelta*(_speed +1);
		rx.eulerAngles = ex;
		_cacheTransform.rotation = rx;
	}
	
	//Crash avoidance //Checks for obstacles forward
	if (Physics.Raycast(_cacheTransform.position, cacheForward+(cacheRight*Random.Range(-.1, .1)), hit, _spawner._stopDistance, _spawner._avoidanceMask)){
				//Debug.DrawLine(_cacheTransform.position,hit.point);
				d = (_spawner._stopDistance - hit.distance)/_spawner._stopDistance;
				ex.y -= _spawner._avoidSpeed*d*_spawner._newDelta*(_targetSpeed +3);
				rx.eulerAngles = ex;
				_cacheTransform.rotation = rx;
				_speed -= d*_spawner._newDelta*_spawner._stopSpeedMultiplier*_speed;
				if(_speed < 0.01){
					_speed = 0.01;
				}
				return true;
	}else if (Physics.Raycast(_cacheTransform.position, cacheForward+(cacheRight*(_spawner._avoidAngle+_rotateCounterL)), hit, _spawner._avoidDistance, _spawner._avoidanceMask)){
//				Debug.DrawLine(_cacheTransform.position,hit.point);
				d = (_spawner._avoidDistance - hit.distance)/_spawner._avoidDistance;
				_rotateCounterL+=.1;
				ex.y -= _spawner._avoidSpeed*d*_spawner._newDelta*_rotateCounterL*(_speed +1);
				rx.eulerAngles = ex;
				_cacheTransform.rotation = rx;
				if(_rotateCounterL > 1.5)
					_rotateCounterL = 1.5;
				_rotateCounterR = 0;
				return true;
	}else if (Physics.Raycast(_cacheTransform.position, cacheForward+(cacheRight*-(_spawner._avoidAngle+_rotateCounterR)), hit, _spawner._avoidDistance, _spawner._avoidanceMask)){
//			Debug.DrawLine(_cacheTransform.position,hit.point);
				d = (_spawner._avoidDistance - hit.distance)/_spawner._avoidDistance;
				if(hit.point.y < _cacheTransform.position.y){
					ex.y -= _spawner._avoidSpeed*d*_spawner._newDelta*(_speed +1);
				}
				else{
					ex.x += _spawner._avoidSpeed*d*_spawner._newDelta*(_speed +1);
				}
				_rotateCounterR +=.1;
				ex.y += _spawner._avoidSpeed*d*_spawner._newDelta*_rotateCounterR*(_speed +1);
				rx.eulerAngles = ex;
				_cacheTransform.rotation = rx;
				if(_rotateCounterR > 1.5)
					_rotateCounterR = 1.5;
				_rotateCounterL = 0;
				return true;
	}else{
		_rotateCounterL = 0;
		_rotateCounterR = 0;
	}
	return false;
	}

function ForwardMovement(){
	_cacheTransform.position += _cacheTransform.TransformDirection(Vector3.forward)*_speed*_spawner._newDelta;
	if (tParam < 1) {
		if(_speed > _targetSpeed){
			tParam += _spawner._newDelta * _spawner._acceleration;
		}else{
			tParam += _spawner._newDelta * _spawner._brake;
		}
		_speed = Mathf.Lerp(_speed, _targetSpeed,tParam);
	}
}

function RotationBasedOnWaypointOrAvoidance (){
	var rotation:Quaternion;
    rotation = Quaternion.LookRotation(_wayPoint - _cacheTransform.position);
    if(!Avoidance()){
		_cacheTransform.rotation = Quaternion.Slerp(_cacheTransform.rotation, rotation, _spawner._newDelta * _damping);
	}
	//Limit rotation up and down to avoid freaky behavior
	var angle:float = _cacheTransform.localEulerAngles.x;
    angle = (angle > 180) ? angle - 360 : angle;
	var rx:Quaternion = _cacheTransform.rotation;
    var rxea:Vector3 = rx.eulerAngles;
    rxea.x = ClampAngle(angle, -50.0f , 50.0f);
    rx.eulerAngles = rxea;
	_cacheTransform.rotation = rx;
}

function CheckForDistanceToWaypoint(){
	if((_cacheTransform.position - _wayPoint).magnitude < _spawner._waypointDistance+_stuckCounter){
      	Wander(0);	//create a new waypoint
        _stuckCounter=0;
        CheckIfThisShouldTriggerNewFlockWaypoint();
        return;
    }
    _stuckCounter+=_spawner._newDelta*(_spawner._waypointDistance*.25);
}

function CheckIfThisShouldTriggerNewFlockWaypoint(){
	if(_spawner._childTriggerPos){
		_spawner.SetRandomWaypointPosition();
	}
}

static function ClampAngle (angle : float, min : float, max : float):float {
	if (angle < -360)angle += 360;
	if (angle > 360)angle -= 360;
	return Mathf.Clamp (angle, min, max);
}

function SetAnimationSpeed(){
	for (var state : AnimationState in _model.GetComponent.<Animation>()) {
		state.speed = (Random.Range(_spawner._minAnimationSpeed, _spawner._maxAnimationSpeed)*_spawner._schoolSpeed*this._speed)+.1;   		   
	}
}

function Wander(delay:float){
	_damping = Random.Range(_spawner._minDamping, _spawner._maxDamping);
	_targetSpeed = Random.Range(_spawner._minSpeed, _spawner._maxSpeed)*_spawner._speedCurveMultiplier.Evaluate(Random.value)*_spawner._schoolSpeed;
	Invoke("SetRandomWaypoint", delay);
}

function SetRandomWaypoint(){
	tParam = 0;
	_wayPoint = findWaypoint();
}