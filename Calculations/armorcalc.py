import matplotlib.pyplot as plt
import numpy as np

MaxLevel = 300
TargetResistance = 0.9

MaterialTiers = 4
ScalingSwitchLevel = 100
DefensePerAbilityLevel = 0.02
TimeShieldPerLevel = 0.02
DefensePerArmorPoint = 0.01
MaxResistance = 0.9

materialBonusPerLevel = pow(1.5, 4 / ScalingSwitchLevel)

def abilityDefMulti(lvl):
    return 1 + DefensePerAbilityLevel * lvl

def qualityBonusAtLevel(lvl):
    if (lvl < 150):
        perLevel = (pow(1.25, 4) - 1) / 150
        return 1.0 + lvl * perLevel
    else:
        bonusBase = pow(1.25, 1 / 37.5)
        return pow(bonusBase, lvl)
    
def itemTierMultiAtLevel(lvl):
    return 1 + (0.2 / ScalingSwitchLevel * min(ScalingSwitchLevel, lvl))

def timeShieldMultiAtLevel(lvl):
    return 1 + 2 * (lvl - 1) * TimeShieldPerLevel

def expectedDefMulti(lvl):
    ability = abilityDefMulti(lvl)
    material = pow(materialBonusPerLevel, min(lvl, ScalingSwitchLevel))
    tier = itemTierMultiAtLevel(lvl)
    quality = qualityBonusAtLevel(lvl)
    shield = timeShieldMultiAtLevel(lvl)
    return ability * material * tier * quality * shield

def MobArmorPierceAtLevel(lvl, damage = 1.0):
    return damage * pow(expectedDefMulti(lvl), 0.5)

def calculateArmorMultiAtLevel(armor, lvl, damage = 1.0):
    if (damage == 0):
        damage = 1
    effectiveArmor = armor / MobArmorPierceAtLevel(lvl, damage)
    return min(1 + effectiveArmor * DefensePerArmorPoint, 1 / (1 - MaxResistance))

def calculateRequiredArmor(lvl, target, damage):
    pierce = MobArmorPierceAtLevel(lvl, damage)
    targetMulti = 1 / (1 - target)
    return targetMulti / DefensePerArmorPoint * pierce


xpoints = range(1, MaxLevel, 1)
# ypoints = list(map(lambda lvl: calculateRequiredArmor(lvl, TargetResistance, 1.0), xpoints))
ypoints = list(map(lambda lvl: MobArmorPierceAtLevel(lvl), xpoints))
ycomp = list(map(lambda lvl: 1 + 1.1 / 100 * (pow(lvl, 1.5) - 1) , xpoints))

plt.plot(xpoints, ypoints)
plt.plot(xpoints, ycomp)
plt.xlabel("Level")
plt.ylabel("Required armor for 90% resistance")
plt.grid('on')
plt.show()