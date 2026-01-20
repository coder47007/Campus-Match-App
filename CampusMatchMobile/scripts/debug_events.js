
const { createClient } = require('@supabase/supabase-js');

// Config
const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'sb_publishable_SVoY0tbQgimNk8Wra5ztCA_cj3UEaNT';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);

async function debugEvents() {
    console.log('--- Debugging Event Creation ---');

    // 1. Check if Events table exists
    console.log('\n1. Checking Events table...');
    const { data: events, error: tableError } = await supabase
        .from('Events')
        .select('*')
        .limit(1);

    if (tableError) {
        console.error('❌ Error accessing Events table:', tableError.message);
        if (tableError.code === '42P01') console.error('   Hint: Table might not exist.');
    } else {
        console.log('✅ Events table is accessible.');
    }

    // 2. Check if Guest Student 1000 exists (Fallback ID)
    console.log('\n2. Checking for Guest Student (ID 1000)...');
    const { data: guest, error: guestError } = await supabase
        .from('Students')
        .select('Id, Name')
        .eq('Id', 1000)
        .single();

    if (guestError || !guest) {
        console.error('❌ Guest Student (ID 1000) NOT FOUND.');
        console.log('   This will cause FK violations if getStudentId returns 1000.');

        // Attempt to create it
        console.log('   Attempting to create Guest ID 1000...');
        const { error: createError } = await supabase
            .from('Students')
            .insert({
                Id: 1000,
                Email: 'guest@campusmatch.com',
                Name: 'Guest User',
                PasswordHash: 'guest',
                PhoneNumber: '000-000-0000',
                CreatedAt: new Date().toISOString(),
                LastActiveAt: new Date().toISOString()
            });

        if (createError) console.error('   ❌ Failed to create guest:', createError.message);
        else console.log('   ✅ Guest user created successfully.');
    } else {
        console.log('✅ Guest Student found:', guest);
    }

    // 3. Try to Create an Event
    console.log('\n3. Test Event Creation...');
    const testEvent = {
        CreatorId: 1000, // Try using the guest ID
        Title: 'Test Event ' + new Date().toISOString(),
        Location: 'Debug Script',
        StartTime: new Date().toISOString(),
        EndTime: new Date(Date.now() + 3600000).toISOString(),
        CreatedAt: new Date().toISOString()
    };

    const { data: newEvent, error: insertError } = await supabase
        .from('Events')
        .insert(testEvent)
        .select()
        .single();

    if (insertError) {
        console.error('❌ Insert Failed:', insertError.message);
        console.error('   Details:', insertError);
    } else {
        console.log('✅ Event Created Successfully:', newEvent);
        // Clean up
        await supabase.from('Events').delete().eq('Id', newEvent.Id);
    }
}

debugEvents();
