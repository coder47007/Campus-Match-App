# Non-Functional Buttons and Links Audit

## 🔍 **Comprehensive Audit Results**

**Date**: January 19, 2026  
**Screens Audited**: 15+  
**Non-Functional Elements Found**: 22

---

## 🚨 **Critical Issues** (Buttons with NO functionality)

### 1. **discover.tsx** - Line 209
**Element**: Profile/Person Icon (Header Right)  
**Current State**: `<TouchableOpacity style={styles.headerIcon}>`  
**Issue**: No `onPress` handler  
**Expected**: Should navigate to profile or show notifications  
**Priority**: HIGH

---

### 2. **matches.tsx** - Lines 135, 141, 168, 174
**Elements**: Header Icons (Options & Person icons)  
**Current State**: Empty `TouchableOpacity` elements  
**Issue**: No `onPress` handlers on 4 buttons  
**Expected**:  
- Line 135, 168: Options button should show filter/settings
- Line 141, 174: Person icon should navigate to profile  
**Priority**: HIGH

---

### 3. **likes.tsx** - Lines 60, 95, 127, 133, 149
**Elements**: Multiple non-functional buttons  

#### Line 60-63: Like Card Click
```tsx
onPress={() => {
    if (!showBlur) {
        // Navigate to profile or match
    }
}
```
**Issue**: Empty function - does nothing  
**Expected**: Navigate to profile or match

#### Line 95: "Upgrade to see" Button
```tsx
<TouchableOpacity style={styles.upgradeButton}>
    <Text style={styles.upgradeText}>Upgrade to see</Text>
</TouchableOpacity>
```
**Issue**: No `onPress` handler  
**Expected**: Show premium/subscription modal

#### Lines 127, 133: Header Icons
**Issue**: No `onPress` handlers  
**Expected**: Navigate to options/profile

#### Line 149: Premium CTA Button
```tsx
<TouchableOpacity style={styles.premiumCta}>
```
**Issue**: No `onPress` handler  
**Expected**: Show subscription page  
**Priority**: CRITICAL (Premium revenue feature!)

---

### 4. **explore.tsx** - Lines 145, 180, 240
**Elements**: Header icon and event cards  

#### Line 145: Header Icon
**Issue**: No `onPress` handler  
**Expected**: Navigate somewhere

#### Line 180: Event Action
```tsx
onPress={() => {
    // TODO: Join/RSVP
}}
```
**Issue**: Empty TODO  
**Expected**: RSVP to event

#### Line 240: Event Card
```tsx
<TouchableOpacity key={event.Id} style={styles.eventCard}>
```
**Issue**: No `onPress` handler  
**Expected**: Show event details  
**Priority**: MEDIUM

---

### 5. **chat/[matchId].tsx** - Lines 245, 307, 319
**Elements**: Emoji, Attach, and GIF buttons  

#### Line 245: GIF Button
```tsx
onPress={() => {
    // TODO: GIF picker
}}
```
**Issue**: Empty TODO  
**Expected**: Show GIF picker

#### Line 307: Attach Button
```tsx
<TouchableOpacity style={styles.attachButton}>
    <Ionicons name="add-circle" .../>
</TouchableOpacity>
```
**Issue**: No `onPress` handler  
**Expected**: Show attachment options (photo, file, location)

#### Line 319: Emoji Button
```tsx
<TouchableOpacity style={styles.emojiButton}>
    <Ionicons name="happy-outline" .../>
</TouchableOpacity>
```
**Issue**: No `onPress` handler  
**Expected**: Show emoji picker  
**Priority**: MEDIUM (Nice-to-have features)

---

### 6. **profile.tsx** - Lines 409, 605, 645, 680
**Elements**: Interest selection buttons  

#### Lines 409, 605, 645, 680: Interest Pills
```tsx
onPress={() => {
    // TODO: Select interest
}}
```
**Issue**: All have empty functions with TODO comments  
**Expected**: Toggle interest selection  
**Priority**: HIGH (Profile editing is core feature)

---

### 7. **login.tsx** - Line 136
**Element**: Demo credentials button  
```tsx
onPress={() => {
    // Auto-fill demo credentials
}}
```
**Issue**: Empty comment but no actual implementation... wait, let me check this more carefully.

---

##⚠️ **Medium Priority** (Links without navigation)

### 8. **auth/register.tsx** - Line 100
**Element**: Back Button (in Link wrapper)  
**Current State**: `<TouchableOpacity style={styles.backButton}>`  
**Issue**: `<Link>` wraps it but TouchableOpacity has no asChild prop  
**Expected**: Should work as-is if Link wraps properly  
**Priority**: MEDIUM (check if Link works)

### 9. **auth/forgot-password.tsx** - Line 107
**Element**: Back Button  
**Same issue as register.tsx**  
**Priority**: MEDIUM

---

## ✅ **False Positives** (These are actually fine)

### login.tsx - Demo Credentials (Lines 136-140)
After checking, this DOES work - it has the handler a few lines below. ✅

### All password visibility toggles
These all work correctly with `setShowPassword` handlers. ✅

---

## 📊 **Summary by Priority**

### 🔴 **Critical** (Revenue Impact)
1. `likes.tsx:149` - Premium CTA button (no handler)
2. `likes.tsx:95` - "Upgrade to see" button (no handler)

### 🟠 **High** (Core Feature Impact)
3. `matches.tsx:135,141,168,174` - Header navigation (4 buttons)
4. `discover.tsx:209` - Profile navigation
5. `likes.tsx:60-63` - Like card navigation
6. `likes.tsx:127,133` - Header icons
7. `profile.tsx:409,605,645,680` - Interest selection (4 buttons)
8. `explore.tsx:180,240` - Event interaction

### 🟡 **Medium** (Nice-to-Have Features)
9. `chat/[matchId].tsx:245,307,319` - Chat enhancements (GIF, attach, emoji)
10. `explore.tsx:145` - Header icon

---

## 🛠 **Recommended Fixes**

### Quick Wins (< 1 hour total)
1. **Add header icon handlers** (10 min)
   - All header icons should navigate to profile or settings
   - Use: `onPress={() => router.push('/profile')}`

2. **Premium buttons** (15 min)
   - Create premium modal or navigate to subscription page
   - Use: `onPress={() => setShowPremiumModal(true)}`

3.** Like card navigation** (5 min)
   - Navigate to profile: `router.push(`/profile/${like.id}`)`

### Medium Effort (1-2 hours)
4. **Interest selection in profile** (30 min)
   - Implement toggle logic for selecting interests

5. **Event interactions** (30 min)
   - RSVP functionality
   - Event detail view

### Nice-to-Have (2-4 hours)
6. **Chat enhancements** (2 hours)
   - Emoji picker implementation
   - Attachment handler
   - GIF picker

---

## 📝 **Implementation Priority Order**

**Phase 1**: Fix critical revenue-impacting buttons
- Premium CTA buttons (likes.tsx)

**Phase 2**: Fix core navigation
- Header icons across all tabs
- Like card navigation

**Phase 3**: Complete profile editing
- Interest selection functionality

**Phase 4**: Polish features
- Chat enhancements
- Event interactions

---

## 🎯 **Total Impact**

- **Buttons to Fix**: 22
- **Estimated Time**: 4-6 hours for all fixes
- **Quick Wins**: ~1 hour for 80% of navigation issues

**Current User Impact**:
- Users see clickable buttons that do nothing (frustrating UX)
- Premium conversion likely affected (upgrade buttons don't work!)
- Profile editing incomplete (can't select interests)

---

**Recommendation**: Start with Phase 1 & 2 (Premium + Navigation) for maximum impact with minimal time investment (~90 minutes).
