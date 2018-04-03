# Homework_2

## 1.简答题

### 1）游戏对象运动的本质是什么？
-  游戏对象的运动就是通过调用相关的API在每一帧变化的过程中改变对象的Transform属性中的position、rotation等属性,使得游戏对象看起来像是在运动。

### 2）请用三种方法以上方法，实现物体的抛物线运动。（如，修改Transform属性，使用向量Vector3的方法…）
    
- 这里我们使用xoy平面上的二次函数y = -x^2 + 5x来模拟抛物线的轨迹，Start()方法中先设置对象的初始位置为原点
    
    - 方法一：先算出x、y方向的偏移量，再改变this.transform.position的值
    ```
    public class parabola_1 : MonoBehaviour {

    	public float speed;
    	// Use this for initialization
    	void Start () {
    		this.transform.position = Vector3.zero;
    	}
    	
    	// Update is called once per frame
    	void Update () {
    		float rightOffset = speed * Time.deltaTime;
    		float previousX = this.transform.position.x;
    		float currentX = previousX + rightOffset;
    		float upOffset = speed * (FunctionValue (currentX) - FunctionValue (previousX));
    
    		this.transform.position += Vector3.right * rightOffset;
    		this.transform.position += Vector3.up * upOffset;
    	}
    
    	//return the y-position of the function y = x(5-x) when x is given 
    	float FunctionValue(float x) {
    		return x * (5 - x);
    	}
    }
    ```
    - 方法二：调用Transform.Translate()方法，其中传入的三个参数是移动方向向量的三个坐标值
    ```
    public class parabola_2 : MonoBehaviour {

    	public float speed;
    	// Use this for initialization
    	void Start () {
    		this.transform.position = Vector3.zero;
    	}
    	
    	// Update is called once per frame
    	void Update () {
    		float rightOffset = speed * Time.deltaTime;
    		float previousX = this.transform.position.x;
    		float currentX = previousX + rightOffset;
    		float upOffset = speed * (FunctionValue (currentX) - FunctionValue (previousX));
    
    		this.transform.Translate (rightOffset, upOffset, 0);
    	}
    
    	//return the y-position of the function y = x(5-x) when x is given 
    	float FunctionValue(float x) {
    		return x * (5 - x);
    	}
    }

    ```
    - 方法三：调用Vector3.MoveTowards()方法，传入的参数是每次的初始位置和目标位置以及移动的步幅。
    
    ```
    public class parabola_3 : MonoBehaviour {

    	public float speed;
    	// Use this for initialization
    	void Start () {
    		this.transform.position = Vector3.zero;
    	}
    
    	// Update is called once per frame
    	void Update () {
    		float step = speed * Time.deltaTime;
    
    		float rightOffset = step;
    		float previousX = this.transform.position.x;
    		float currentX = previousX + rightOffset;
    		float upOffset = FunctionValue (currentX) - FunctionValue (previousX);
    		float previousY = this.transform.position.y;
    		float currentY = previousY + upOffset;
    		Vector3 target = new Vector3 (currentX, currentY, this.transform.position.z);
    
    		this.transform.position = Vector3.MoveTowards (this.transform.position, target, step);
    	}
    
    	//return the y-position of the function y = x(5-x) when x is given 
    	float FunctionValue(float x) {
    		return x * (5 - x);
    	}
    }

    ```
### 3）写一个程序，实现一个完整的太阳系， 其他星球围绕太阳的转速必须不一样，且不在一个法平面上。

- 代码如下
```
//行星公转脚本
public class RotateAroundPlanet : MonoBehaviour {

	public Transform origin;
	public float speed;
	float axisX, axisY;
	// Use this for initialization
	void Start () {
		axisX = Random.Range (-3,3);
		axisY = Random.Range (15, 20);
	}

	// Update is called once per frame
	void Update () {
		Vector3 axis = new Vector3(axisX, axisY, 0);  
		this.transform.RotateAround(origin.position, axis, speed * Time.deltaTime);  
	}
}

//地球自转脚本
public class SelfRotate : MonoBehaviour {

	public float selfSpeed; 
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		this.transform.RotateAround(this.transform.position, Vector3.up, selfSpeed * Time.deltaTime);
	}
}

//地球的影子自转脚本（用来作为月球的父对象，它的位置和地球一直相同，运行时将地球赋值给trans）
public class ShadeOfEarth : MonoBehaviour {

	public Transform trans;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = trans.position;	
	}
}



```

## 2、编程实践

### 1）列出游戏中提及的事物（Objects）
  - 牧师
  - 魔鬼
  - 船
  - 河岸
  - 水
### 2）用表格列出玩家动作表
  - 游戏开始时有两个河岸隔着一条河，一只船停靠在岸边，停靠的岸上有三个牧师和三个魔鬼
  - 玩家可以点击人物以移动人物（牧师或魔鬼），使其上船（下岸）或下船（上岸）
  - 点击船只来移动船只，移动时船上必须有至少一个人驾驶，至多两个
  - 若有一岸边魔鬼的数量（包括岸边船上的魔鬼）大于牧师的数量（包括岸边船上的牧师），则魔鬼吃掉牧师游戏结束，点击重新开始
  - 成功将六个人物安全送到对岸则获得胜利
