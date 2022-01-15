import random

# 무기 공격력 1 ~ 15
def getDefaultDamage(weaponDamage, hitRating):
    result = {}
    sum = 0
    if hitRating <= 100:
        weight = [1 - hitRating / 100 if damage < weaponDamage / 2 else 0 + hitRating / 100 for damage in range(1, weaponDamage + 1) ]
    elif hitRating <= 200:
        weight = [1 - hitRating / 100 if damage < weaponDamage / 2 else 0 + hitRating / 100 for damage in range(1, weaponDamage + 1) ]
    # hitRating / 50
    # print(weight)
    # print(type(weight))
    # sum = 0
    # for damage in weight:
    #     sum += damage
    # print(sum)
    # print(sum / len(weight))
    # print(damage = random.choices(range(1, weaponDamage + 1), weight))
    for _ in range(10000):
        damage = random.choices(range(1, weaponDamage + 1), weight)[0]
        if damage in result:
            result[damage] += 1
        else:
            result[damage] = 1
        sum += damage

    return result, sum/10000, weight

# print(getDefaultDamage(15, 0))
# print(getDefaultDamage(15, 10)[1])
# print(getDefaultDamage(15, 20)[1])
# print(getDefaultDamage(15, 30)[1])
# print(getDefaultDamage(15, 40)[1])
# print(getDefaultDamage(15, 50))
# print(getDefaultDamage(15, 60)[1])
# print(getDefaultDamage(15, 70)[1])
# print(getDefaultDamage(15, 80)[1])
# print(getDefaultDamage(15, 90)[1])
# print(getDefaultDamage(15, 100))

# ({4: 1448, 3: 1437, 2: 1420, 1: 1371, 6: 1375, 5: 1515, 7: 1434}, 4.0177, [1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0])
# 4.7768
# 5.6818
# 6.4607
# 7.2774
# ({4: 701, 8: 644, 7: 678, 2: 655, 6: 631, 13: 674, 15: 646, 9: 649, 10: 633, 14: 676, 3: 658, 5: 670, 12: 676, 11: 709, 1: 700}, 7.982, [0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5])
# 8.6491
# 9.4058
# 10.163
# 10.8393
# ({8: 1350, 9: 1317, 13: 1268, 10: 1241, 11: 1181, 12: 1260, 14: 1196, 15: 1187}, 11.4207, [0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0])

def calculAverageDamage(defaultDamage, minDamage, maxDamage, attackSpeed, criticalPercent):
    total = 0
    for _ in range(1000):
        rand = random.random()
        if rand < criticalPercent:
            total += (maxDamage + defaultDamage) * (100 + attackSpeed) / 100
        else:
            total += (random.choices(range(minDamage, maxDamage + 1))[0] + defaultDamage) * (100 + attackSpeed) / 100
    
    return total / 1000

# 일반
# 2 / 3.5
# print(calculAverageDamage(0, 1, 2, 20, 0)) # 단검
# print(calculAverageDamage(1, 1, 1, 10, 0)) # 한손검
# print(calculAverageDamage(2, 1, 2, 0, 0))  # 양손검
# print(calculAverageDamage(0, 1, 6, 0, 0))  # 양손도끼
# print()

# 고급
# 3.5 / 6.0
# print(calculAverageDamage(1, 1, 3, 20, 0)) # 단검
# print(calculAverageDamage(2, 1, 2, 10, 0)) # 한손검
# print(calculAverageDamage(3, 1, 5, 0, 0))  # 양손검
# print(calculAverageDamage(0, 1, 11, 0, 0))  # 양손도끼
# print()

# 고급 #2
# 5.0 / 8.0
# print(calculAverageDamage(1, 2, 4, 20, 0)) # 단검
# print(calculAverageDamage(2, 2, 3, 10, 0)) # 한손검
# print(calculAverageDamage(4, 2, 6, 0, 0))  # 양손검
# print(calculAverageDamage(0, 2, 15, 0, 0))  # 양손도끼 (드랍)
# print()

# 희귀 #1
# 8.0 / 12.0
# print(calculAverageDamage(3, 3, 4, 30, 0)) # 단검
print(calculAverageDamage(5, 2, 3, 15, 0)) # 한손검
# print(calculAverageDamage(5, 5, 8, 0, 0))  # 양손검
# print(calculAverageDamage(1, 8, 0, 0))  # 양손도끼
# print()

# 희귀 #2
# print(calculAverageDamage(1, 1, 10, 20, 0.2)) # 단검
print(calculAverageDamage(3, 3, 10, 10, 0)) # 한손검
# print(calculAverageDamage(14, 0, 0, 0, 0))  # 양손검


print(calculAverageDamage(0, 1, 20, 15, 0)) 
print(calculAverageDamage(0, 1, 30, 0, 0)) 