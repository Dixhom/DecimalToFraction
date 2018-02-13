# DecimalToFraction

DecimalToFraction is a C# console application to approximate a decimal number by fractions.

# Example

Approximate pi(3.1415) by fractions whose denominators are from 2 to 9.

```
C:\DecimalToFraction> DecimalToFraction.exe 3.1415 2 9
[approximated fractions of 3.1415]
(In order of denominator)
---------------------------------------------------
13/4    (error : 3.45%)
16/5    (error : 1.86%)
19/6    (error : 0.801%)
22/7    (error : 0.0432%)
25/8    (error : -0.525%)
28/9    (error : -0.967%)

[The closest fraction]
22/7    (error : 0.0432%)

Press any key to exit...
```

# Detail

Irrational numbers such as pi can't be expressed exactly as a common fraction. However, it can still be approximated by 22/7 and other rational numbers. 
Then, what are those rational numbers? How close are they to pi? DecimalToFraction answers those questions. This can approximate a decimal number by
fractions whose denominators are in any range you want. This comes quite in handy when you need to get a rational approximation of an irrational number.

# Specification

- Fractions with denominators being powers of 10 are not shown
(3/1, 31/10, 314/100 for pi don't make sense)
- Reducible fractions are not shown 

# Todo

- more options
	- `--digit 3` for all 3 digit denominators
	- `--range 2 99` for denominator range 2 to 99
	- `--numerator` for numerator range

