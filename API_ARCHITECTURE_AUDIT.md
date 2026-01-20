# Phase 3.2: API Architecture Audit Report

## Direct Supabase Database Queries Found

### ❌ Issue: Client Bypassing Backend API

The mobile client currently makes **direct database queries** to Supabase, bypassing the backend API entirely. This creates several problems:

1. **Security Risk**: Client has direct database access
2. **No Validation**: Bypasses backend business logic
3. **Inconsistent Patterns**: Some features use REST API, others use direct queries
4. **Hard to Monitor**: Can't track/log these queries from backend

---

## Findings

### 1. Direct Database DELETE in auth.ts
**File**: [`services/auth.ts:209`](file:///d:/College/Projects/Mobile%20Apps/CampusMatch%20(2)/CampusMatchMobile/services/auth.ts#L209)

**Current Code**:
```typescript
await supabase.from('Students').delete().eq('Id', user.id);
```

**Issue**: Deletes student record directly from database without backend validation

**Recommended Fix**: Create backend endpoint
```typescript
// Backend: DELETE /api/profiles/me
[HttpDelete("me")]
public async Task<IActionResult> DeleteAccount()
{
    var userId = GetUserId();
    
    // Add business logic: Delete related data, send notifications, etc.
    var student = await _db.Students.FindAsync(userId);
    if (student == null) return NotFound();
    
    _db.Students.Remove(student);
    await _db.SaveChangesAsync();
    
    return NoContent();
}

// Client: Use REST API instead
await api.delete('/profiles/me');
```

---

### 2. Direct Database QUERY in supabase.ts
**File**: [`services/supabase.ts:63-67`](file:///d:/College/Projects/Mobile%20Apps/CampusMatch%20(2)/CampusMatchMobile/services/supabase.ts#L63-L67)

**Current Code**:
```typescript
const { data, error } = await supabase
    .from('Students')
    .select('Id')
    .eq('Email', user.email)
    .single();
```

**Issue**: Queries Students table directly to get student ID

**Status**: ⚠️ **PARTIALLY ACCEPTABLE**  
This is used only during authentication to map Supabase user → Student ID. Since it's a one-time lookup at login, it's relatively safe, but should still be moved to backend for consistency.

**Recommended Fix**: Create backend endpoint
```csharp
// Backend: GET /api/auth/student-id
[HttpGet("student-id")]
[AllowAnonymous] // Allow during login process
public async Task<ActionResult<int>> GetStudentIdByEmail([FromQuery] string email)
{
    var student = await _db.Students
        .Where(s => s.Email == email)
        .Select(s => s.Id)
        .FirstOrDefaultAsync();
        
    if (student == 0) return NotFound();
    return Ok(student);
}
```

---

## Summary

| Location | Query Type | Severity | Status |
|----------|------------|----------|--------|
| `auth.ts:209` | DELETE Students | 🔴 HIGH | Must Fix |
| `supabase.ts:63` | SELECT Students.Id | 🟡 MEDIUM | Should Fix |

---

## Recommendation

### Phase 3.2 Action Plan:

1. **Create Missing Backend Endpoints** ✅
   - `DELETE /api/profiles/me` - Delete account
   - `GET /api/auth/student-id` - Get student ID by email (optional)

2. **Update Client Code**
   - Replace direct Supabase calls with REST API calls
   - Keep Supabase ONLY for authentication

3. **Test**
   - Verify account deletion works
   - Verify login flow still works

---

## Benefits After Fix

✅ **Security**: All database access goes through backend validation  
✅ **Consistency**: Single API pattern throughout app  
✅ **Monitoring**: Can log/track all data access  
✅ **Business Logic**: Can add hooks (send emails, clean up data, etc.)  

---

**Next Step**: Implement missing endpoints and update client code.
