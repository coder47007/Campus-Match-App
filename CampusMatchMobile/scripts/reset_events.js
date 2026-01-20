
const { createClient } = require('@supabase/supabase-js');

// Config
const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'sb_publishable_SVoY0tbQgimNk8Wra5ztCA_cj3UEaNT';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);

async function resetEvents() {
    console.log('--- Wiping All Events ---');

    const { error } = await supabase
        .from('Events')
        .delete()
        .neq('Id', 0); // Delete all where Id is not 0 (effectively all)

    if (error) {
        console.error('❌ Error deleting events:', error.message);
    } else {
        console.log('✅ All events deleted. Start fresh!');
    }
}

resetEvents();
