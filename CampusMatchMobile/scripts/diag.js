
const { createClient } = require('@supabase/supabase-js');

// Configuration
const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);

async function diag() {
    console.log('--- Diagnostic Start ---');

    // 1. Check Buckets
    console.log('\nChecking Storage Buckets...');
    const { data: buckets, error: bucketError } = await supabase.storage.listBuckets();
    if (bucketError) console.error('Bucket Error:', bucketError.message);
    else console.log('Buckets:', buckets ? buckets.map(b => b.name) : 'None');

    // 2. Check Table 'Students' (Capitalized)
    console.log("\nChecking table 'Students'...");
    const { data: d1, error: e1 } = await supabase.from('Students').select('*').limit(1);
    if (e1) console.error('Error selecting Students:', e1.message);
    else console.log("Success 'Students':", d1);

    // 3. Check Table 'students' (Lowercase)
    console.log("\nChecking table 'students'...");
    const { data: d2, error: e2 } = await supabase.from('students').select('*').limit(1);
    if (e2) console.error('Error selecting students:', e2.message);
    else console.log("Success 'students':", d2);

    console.log('\n--- Diagnostic End ---');
}

diag();
