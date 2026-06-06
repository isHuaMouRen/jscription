# Math 系列命令

[主页](/docs/home.md)

Math 系列命令用于数学运算

## math.abs

求绝对值

|参数|类型|描述|
|-|-|-|
| `value` |double|值|

|返回值类型|描述|
|-|-|
|double|绝对值|

## math.max

取两个数中最大的那个

|参数|类型|描述|
|-|-|-|
| `value1` |double|值1|
| `value2` |double|值2|

|返回值类型|描述|
|-|-|
|double|较大的那个值|

## math.min

取两个数中最小的那个

|参数|类型|描述|
|-|-|-|
| `value1` |double|值1|
| `value2` |double|值2|

|返回值类型|描述|
|-|-|
|double|较小的那个值|

## math.sign

返回值的符号，正数返回 `1` ，负数返回 `-1` ，零返回 `0`

|参数|类型|描述|
|-|-|-|
| `value` |double|值|

|返回值类型|描述|
|-|-|
|int|符号|

## math.clamp

将值限定在指定范围内，如未超出则返回原值，如超出则返回边界值

|参数|类型|描述|
|-|-|-|
| `value` |double|值|
| `max` |double|最小值|
| `min` |double|最大值|

|返回值类型|描述|
|-|-|
|double|结果|

## math.ceiling

向上取整，如 $1.3 \rightarrow 2$

|参数|类型|描述|
|-|-|-|
| `value` |double|值|

|返回值类型|描述|
|-|-|
|double|结果|

## math.floor

向下取整，如 $1.6 \rightarrow 1$

|参数|类型|描述|
|-|-|-|
| `value` |double|值|

|返回值类型|描述|
|-|-|
|double|结果|

## math.round

四舍五入，如 $1.6 \rightarrow 2 \ 1.3 \rightarrow 1$

|参数|类型|描述|
|-|-|-|
| `value` |double|值|

|返回值类型|描述|
|-|-|
|double|结果|

## math.truncate

截断取整，如 $1.9 \rightarrow 1$

|参数|类型|描述|
|-|-|-|
| `value` |double|值|

|返回值类型|描述|
|-|-|
|double|结果|
