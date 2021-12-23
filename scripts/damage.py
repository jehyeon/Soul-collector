import random

# 무기 공격력 1 ~ 15
def getDefaultDamage(weaponDamage, hitRating):
    result = {}
    sum = 0
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

    return result, sum/10000

print(getDefaultDamage(15, 0))
print(getDefaultDamage(15, 10)[1])
print(getDefaultDamage(15, 20)[1])
print(getDefaultDamage(15, 30)[1])
print(getDefaultDamage(15, 40)[1])
print(getDefaultDamage(15, 50)[1])
print(getDefaultDamage(15, 60)[1])
print(getDefaultDamage(15, 70)[1])
print(getDefaultDamage(15, 80)[1])
print(getDefaultDamage(15, 90)[1])
print(getDefaultDamage(15, 100))
